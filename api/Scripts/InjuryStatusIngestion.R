dbPath <- "C:/Code/Football-GM/api/footballgm.dev.db"
   con <- dbConnect(SQLite(),dbPath)

if (!requireNamespace("DBI", quietly = TRUE)) install.packages("DBI")
if (!requireNamespace("RSQLite", quietly = TRUE)) install.packages("RSQLite")
if (!requireNamespace("nflreadr", quietly = TRUE)) install.packages("nflreadr")

library(DBI)
library(RSQLite)
library(nflreadr)
library(dplyr)

all_injuries <- load_injuries(most_recent_season())

off_injuries <- all_injuries[all_injuries$position %in% c("QB", "RB", "WR", "TE", "K", "P"),]

injuries <- data.frame(
	GameId = off_injuries$season,
	Week = off_injuries$week,
	PlayerId = off_injuries$gsis_id,
	OfficialReportStatus = off_injuries$report_primary_injury,
	PracticePrimaryStatus = off_injuries$practice_primary_injury,
	PracticeStatus = off_injuries$practice_status
)
		
dbWriteTable(con, "InjuryStatus", injuries, overwrite = TRUE)

dbDisconnect(con)
