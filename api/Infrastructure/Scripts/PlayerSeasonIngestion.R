dbPath <- "C:/Code/Football-GM/api/footballgm.dev.db"
   con <- dbConnect(SQLite(),dbPath)

if (!requireNamespace("DBI", quietly = TRUE)) install.packages("DBI")
if (!requireNamespace("RSQLite", quietly = TRUE)) install.packages("RSQLite")
if (!requireNamespace("nflreadr", quietly = TRUE)) install.packages("nflreadr")

library(DBI)
library(RSQLite)
library(nflreadr)
library(dplyr)

playerSeason <- playerGame %>% group_by(Season, PlayerId) %>%
    summarise(
        PassAttempts = sum(PassAttempts, na.rm = TRUE),
        PassCompletions = sum(PassCompletions, na.rm = TRUE),
        PassingYards = sum(PassingYards, na.rm = TRUE),
        PassingTouchdowns = sum(PassingTouchdowns, na.rm = TRUE),
        RushAttempts = sum(RushAttempts, na.rm = TRUE),
        RushingYards = sum(RushingYards, na.rm = TRUE),
        RushingFirstDowns = sum(RushingFirstDowns, na.rm = TRUE),
        RushingTouchdowns = sum(RushingTouchdowns, na.rm = TRUE),
        Receptions = sum(Receptions, na.rm = TRUE),
        ReceivingYards = sum(ReceivingYards, na.rm = TRUE),
        ReceivingTouchdowns = sum(ReceivingTouchdowns, na.rm = TRUE),
        Interceptions = sum(Interceptions, na.rm = TRUE),
        Fumbles = sum(Fumbles, na.rm = TRUE),
        Sacks = sum(Sacks, na.rm = TRUE),
        FieldGoalsMade = paste(
            FieldGoalsMade[!is.na(FieldGoalsMade) & FieldGoalsMade != ""],
            collapse = ","
        ),
        FieldGoalsMissed = paste(
            FieldGoalsMissed[!is.na(FieldGoalsMissed) & FieldGoalsMissed != ""],
            collapse = ","
        ),
        ExtraPointsMade = sum(ExtraPointsMade, na.rm = TRUE),
        ExtraPointsAttempted = sum(ExtraPointsAttempted, na.rm = TRUE),
        PassingTwoPointConversions = sum(PassingTwoPointConversions, na.rm = TRUE),
        RushingTwoPointConversions = sum(RushingTwoPointConversions, na.rm = TRUE),
        ReceivingTwoPointConversions = sum(ReceivingTwoPointConversions, na.rm = TRUE),
        ReturnedTouchdowns = sum(ReturnedTouchdowns, na.rm = TRUE),
        .groups = "drop"
    )
	
	dbWriteTable(con, "PlayerSeason", playerSeason, overwrite = TRUE)
	dbDisconnect(con)
