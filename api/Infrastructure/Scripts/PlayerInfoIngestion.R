dbPath <- "C:/Code/Football-GM/api/footballgm.dev.db"
   con <- dbConnect(SQLite(),dbPath)

if (!requireNamespace("DBI", quietly = TRUE)) install.packages("DBI")
if (!requireNamespace("RSQLite", quietly = TRUE)) install.packages("RSQLite")
if (!requireNamespace("nflreadr", quietly = TRUE)) install.packages("nflreadr")

library(DBI)
library(RSQLite)
library(nflreadr)
library(dplyr)

all_player_summary <- load_players(most_recent_season())

off_player_info <- all_player_summary[all_player_summary$position %in% c("QB", "RB", "WR", "TE", "K", "P"),]

player_info <- data.frame(
	PlayerId = off_player_info$gsis_id,
	Name = off_player_info$display_name,
	PictureUrl = off_player_info$headshot,
	Team = off_player_info$latest_team,
	Position = off_player_info$position,
	JerseyNumber = off_player_info$jersey_number,
	DraftYear = off_player_info$rookie_season)

dbWriteTable(con, "Player", player_info, overwrite = TRUE)

dbDisconnect(con)
