# Auction Draft

Spec to implement from. Existing auction/REST draft screens, `AuctionOrchestrator`, and polling UI are throwaway. Keep the SignalR playground plumbing (JWT hub auth, `DraftHub` at `/hubs/draft`, Flutter reconnect + re-Join).

## Authority

| Layer | Job |
|---|---|
| **SQLite** | The draft. Survives hours, app closes, and API restarts. |
| **`DraftService`** | Referee. Only place that says yes/no and writes. |
| **`DraftHub`** | One pipe. Commands in, `DraftUpdated` out to group `draft-{leagueId}`. |
| **Flutter** | TV. Replace the snapshot and draw it. Preview is courtesy, not law. |

If the UI and the database disagree, the database wins and the UI snaps to the snapshot.

REST still exists for catalogs (player list when nominating) and auth. Live turns are hub methods only. Do not add a second Auction hub.

## Two machines, one snapshot

```
DRAFT (the room, hours)
  Lobby ──Start──► Live ──rosters full──► Complete
                    │
                    └── Close from Lobby → Closed
                    └── repeats: nominate → auction → settle → next nominator

AUCTION (one NFL player, minutes)
  Nominated (no contract yet, extra time)
    ── first Bid ──► Bidding ── last pass / timeout ──► Settled
    ── first-turn timeout with no Bid ──► Cancelled (player back to pool)
```

`currentAuction == null` → nominate UI.  
`currentAuction != null` → bid/pass UI.  
Same screen, same hub, same `on('DraftUpdated')`.

Reconnect does not pick a player or write a contract. Snapshot says who is up; that person acts.

## Snapshot

After every **successful** command: save SQLite, then

```csharp
await Clients.Group($"draft-{leagueId}").SendAsync("DraftUpdated", snapshot);
```

Rejected commands: error to **caller only**. Snapshot unchanged. Timer does not rewind. Everyone else hears nothing.

`Join` uses the same event: add to group, load snapshot from DB, `Clients.Caller.SendAsync("DraftUpdated", snapshot)`.

Flutter: one notifier. `snapshot = event; notifyListeners();` Throw the old picture away. Do not patch locally. Do not GET the snapshot after a Bid.

Suggested shape (fields appear as tickets need them):

```
DraftSnapshot
  draftId, leagueId
  status                  // Lobby | Live | Complete | Closed
  nominationOrder         // user ids, set at Start
  currentNominatorUserId
  members[]               // userId, displayName, leftoverFunds, connected?
  currentAuction?         // null in lobby / between nominations / after close
    player                // id, name, position, nfl team
    minRating             // from previous seasons; floor for the opening bid
    currentBidderUserId
    highBid?              // duration, salary, bonus, rating, userId — null until first Bid
    passedUserIds[]
    turnStartedAtUtc
    turnSeconds           // longer on the opening bid
  lastEvent               // optional feed line
```

Progress bar is display only: `turnSeconds - (now - turnStartedAtUtc)`. When the bar ends, the phone does **not** call Pass. It waits for `DraftUpdated`.

## Commands

| Command | Who | Body | Server |
|---|---|---|---|
| `Open` | Commissioner | `leagueId` | Create draft, status Lobby |
| `Join` | League member | `leagueId` | Add to group, send snapshot to caller |
| `Close` | Commissioner | — | Lobby → Closed. Room is done. |
| `Start` | Commissioner, from Lobby | nomination order | Status Live, first nominator |
| `Nominate` | Current nominator | `{ playerId }` | Open auction, min rating on snapshot, nominator is current bidder, **no contract yet**, extra time |
| `Bid` | Current bidder | `{ duration, salary, bonus }` | Rate on server. Must beat high rating (or ≥ min if first) and be affordable. Save bid, rotate, reset clock |
| `Pass` | Current bidder, or server clock | — | Mark passed, rotate or settle or cancel empty auction |

Client never sends rating, next bidder, leftover funds, or a snapshot.

### Nominate (two steps)

1. `Nominate({ playerId })` — player free? your turn? Compute min rating from previous seasons. Auction opens with you as current bidder, high bid none. Extra time.
2. You compose Duration / Salary / Bonus and `Bid`. Must meet min rating and leftover funds.

If extra time expires with no legal Bid: Pass with no high bidder. Player returns to the pool. Next nominator. No contract written.

### Bid checks (always on the server)

1. **Better** — computed rating > current high (or ≥ min rating if no bid yet).
2. **Affordable** — cost fits leftover funds.

UI may preview rating and disable Send. Same formula in Dart is a hint. If preview and server disagree, server wins; show the error; leave the form filled; timer keeps running.

**Leftover funds:** starting pool minus Σ `(salary + signingBonus) / duration` for players already won. New offer is affordable if its cost ≤ leftover. No weekly cap schedule during the draft.

### Settle

When the last pass/timeout happens **and there is a high bidder**: one SQLite transaction — insert contract, decrease leftover, clear `currentAuction`, next nominator — then one `DraftUpdated`.

No client button. Winner does not need to be online.

## Layout

```
Flutter
  DraftRoomScreen  ──paints──  DraftSnapshot
  DraftHubClient   ──invoke──  Open / Join / Close / Start / Nominate / Bid / Pass
                   ──listen──  DraftUpdated

ASP.NET
  DraftHub            // auth, group, forward, broadcast
       │
       ▼
  DraftService        // all rules
       ├── DraftRepository
       ├── leftover funds + contracts
       └── clock (turn start timestamps; later a hosted alarm for timeout-as-Pass)
```

Hub stays thin. Test `DraftService` without SignalR.

## Tickets

Do in order. A ticket is done when the **Done when** line is true. Do not start the next ticket early.

---

### Ticket 1 — Open, Join, Close

The room, no football.

**Server**

- `Draft` table: leagueId, status (`Lobby` | `Closed`), timestamps. One open draft per league.
- `DraftService.Open / JoinSnapshot / Close`.
- `DraftHub`: replace playground Increment/Decrement with `Open`, `Join(leagueId)`, `Close`.
- `Join`: must be a league member; add to group `draft-{leagueId}`; send `DraftUpdated` to caller.
- `Open`: commissioner only; persist Lobby; broadcast snapshot (members from `LeagueMember`).
- `Close`: commissioner only; only from Lobby; status Closed; broadcast; further commands fail.

**Client**

- Replace playground screen with a lobby: member names, Open (commissioner), Close (commissioner), Join on enter.
- Listen for `DraftUpdated`. Replace local snapshot. Re-Join on reconnect (already in the playground client).

**Done when:** two users Join a lobby. Commissioner Open (if needed) and Close. Both screens show Closed without a refresh. Kill the Flutter app, Join again, still Closed (or no open draft). Restart the API, same. Playground counter is gone.

---

### Ticket 2 — Start

**Server:** `Start(nominationOrder)` from Lobby, commissioner only. Status Live. Persist order + `currentNominatorUserId` (first in order). Broadcast.

**Client:** Start button for commissioner while Lobby. After Start, show whose turn it is to nominate. No player picker yet.

**Done when:** Start flips every connected client to Live and names the nominator. Reconnect still Live with the same nominator. Close is rejected once Live (or you hide the button).

---

### Ticket 3 — Nominate

**REST:** available players for the league (catalog only).

**Server:** `Nominate(playerId)` — only `currentNominatorUserId`, only when `currentAuction` is null, player not already under contract in this league. Write auction: player, min rating (stub a constant if season math is not ready), nominator as current bidder, no high bid, `turnStartedAtUtc`, extra `turnSeconds`. Broadcast. Do not auto-apply a contract.

**Client:** If you are the nominator and there is no auction, pick a player. Everyone sees that player on the block + min rating. Opening-bid form for the nominator only.

**Done when:** nominating puts the same player on every screen. Reconnect still shows that player and whose Bid turn it is. A non-nominator cannot Nominate.

---

### Ticket 4 — Bid and Pass (no timer yet)

**Server:** `Bid({ duration, salary, bonus })` and `Pass`. Rating computed on the server. Both checks. Rotate among members who have not passed. Rejected Bid → error to caller, no broadcast.

Empty auction + Pass / no high bidder → cancel auction, player back to pool, next nominator.

Auction with a high bidder and only that bidder still active → **do not settle yet**; see Ticket 6. For this ticket, mark auction complete in memory/snapshot enough that Bid/Pass stop, or skip the “last person” path until Ticket 6 if easier — but Pass rotation and Bid save must persist.

**Client:** Bid/Pass enabled only for `currentBidderUserId`. Show high bid and leftover funds from the snapshot. Disable Send when preview looks illegal (optional this ticket).

**Done when:** three users can Bid and Pass, high bid persists across API restart, illegal Bid shows an error only on the bidder’s phone, everyone else unchanged.

---

### Ticket 5 — Server timer

**Server:** hosted clock, not a timer on the hub instance. When `now > turnStartedAtUtc + turnSeconds`, call Pass as the system. Opening bid uses the longer seconds. Persist timestamps so a restart does not reset a turn to full time.

**Client:** progress bar from snapshot fields only. Bar completion does not invoke Pass.

**Done when:** let a turn expire with the apps open; everyone sees the Pass. Let a turn expire with the current bidder’s app killed; still Passes. Rejoin mid-turn; bar shows remaining time, not a fresh bar.

---

### Ticket 6 — Settle

**Server:** on last pass/timeout **with** a high bidder, one transaction: contract, leftover funds, clear auction, next nominator. Then one `DraftUpdated`. Timeout with **no** high bidder stays Ticket 4’s cancel path.

**Done when:** winner’s leftover drops, contract exists in DB, next nominator is prompted, winner can be disconnected and still have the player. Kill the API between “would have been two writes”; you never get a contract without a deduction.

---

### Ticket 7 — Preview and min rating

- Real min rating from previous seasons (replace the Ticket 3 stub).
- Dart preview of rating + leftover so Send can disable.
- Opening-bid extra time already in Ticket 5; tune the numbers as settings if needed.

**Done when:** UI blocks offers under min / over leftover, and a crafted hub Bid that skips the UI is still rejected by the server.

---

### Ticket 8 — End of draft (later)

Rosters full → status Complete. Pause/resume, commissioner abort of a Live draft, weekly cap instead of leftover pool — not required for the room to work.

## Defaults (change later)

- Normal turn: 30s. Opening bid: 60s.
- Starting leftover: league setting or a constant until settings exist.
- Min rating: stub in Ticket 3, real formula in Ticket 7.

## Out of scope for these tickets

- Second SignalR hub
- REST Bid/Pass/Nominate
- Client-owned timer or client-owned leftover arithmetic as truth
- Auto-nominating a player or auto-applying a minimum contract
- Batch `DraftOutcome` at the end of the draft (settle each player when that auction ends)

## Prior sketch (`AuctionOrchestrator` / `AuctionState`)

Throwaway as a product, useful as notes for **Tickets 3–4**. There is no Draft/Lobby here. `StartAuction` is “open this player,” not Open/Join/Start.

**Steal (ideas, rewrite into `DraftService`):**

- Command returns a state DTO (`AuctionState.From(...)`). Same idea as `DraftSnapshot`.
- Guards on Bid/Pass: auction active, it is your turn, you are a member, you have not passed, rating must beat high.
- Round-robin: `GetNextBidderId` walks `AuctionMembers` where `!HasPassed`, ordered by `TurnOrder`. One active bidder left → auction is over.
- Persist `CurrentTurnStartedAtUtc` on each Bid/Pass (Ticket 5 clock).
- Unique auction per `(LeagueId, PlayerId)` while that player is taken.
- Snapshot-ish fields already on `AuctionState`: player identity, current bidder + name, high rating / high bidder, bid history, active vs passed lists.

**Do not copy:**

- Client-supplied `bid.Rating` as truth. Persist Duration / Salary / Bonus; server computes rating.
- `CurrentBidderId = memberIds.First()` on start. First bidder is the **nominator**, and the first turn has **no contract** and extra time.
- Completing the auction as “set Status = Completed” with no contract, leftover, or next nominator.
- `GetNextBidderId` when **zero** people remain (everyone passed, no high bid). Today it only treats `Count == 1` as done; `Count == 0` is unhandled. That path is Ticket 4 cancel (player back to pool).
- Unique index with no room to cancel-and-renominate. Q14 needs Cancelled (or delete) so the player can be nominated again.
- Entity `Bid` storing only Rating; model `Bid` has contract terms that never get saved.
- Broken `ICollection<List<User>> LeagueMembers` on the entity. Ignore it.
