dbPath <- "C:/Code/Football-GM/api/footballgm.dev.db"
   con <- dbConnect(SQLite(),dbPath)

if (!requireNamespace("DBI", quietly = TRUE)) install.packages("DBI")
if (!requireNamespace("RSQLite", quietly = TRUE)) install.packages("RSQLite")
if (!requireNamespace("nflreadr", quietly = TRUE)) install.packages("nflreadr")

library(DBI)
library(RSQLite)
library(nflreadr)
library(dplyr)

all_games <-load_schedules(most_recent_season())

games <- data.frame(
	GameId = all_games$game_id,
	Season = all_games$season,
	Week = all_games$week,
	Date = all_games$gameday,
	HomeTeam = all_games$home_team,
	AwayTeam = all_games$away_team,
	HomeScore = all_games$home_score,
	AwayScore = all_games$away_score,
	WindSpeed = all_games$wind,
	Temperature = all_games$temp
)
	
dbWriteTable(con, "Games", games, overwrite = TRUE)

dbDisconnect(con)
