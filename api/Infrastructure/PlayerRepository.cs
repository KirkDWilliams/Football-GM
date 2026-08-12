using System.Collections.Generic;
using System.Threading.Tasks;
using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Ingested;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Infrastructure
{
    public class PlayerRepository
    {
        private readonly AppDbContext _context;

        public PlayerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Player?> GetPlayerByIdAsync(string playerId)
        {
            var leagues = _context.Leagues
                .Include(l => l.Teams);

            return await _context.Players.FirstOrDefaultAsync(p => p.PlayerId == playerId);
        }

        public async Task<List<Player>> GetPlayersByTeamIdAsync(string teamId)
        {
            return await _context.Players.Where(p => p.TeamId == teamId).OrEmpty().ToListAsync();
        }

        public async Task<List<Player>> GetAllPlayersAsync()
        {
            return await _context.Players.ToListAsync();
        }
    }
}
