
if (!requireNamespace("DBI", quietly = TRUE)) install.packages("DBI")
if (!requireNamespace("RSQLite", quietly = TRUE)) install.packages("RSQLite")
if (!requireNamespace("nflreadr", quietly = TRUE)) install.packages("nflreadr")

library(DBI)
library(RSQLite)
library(nflreadr)

dbPath <- "C:/Code/Football-GM/api/footballgm.dev.db"
con <- dbConnect(RSQLite::SQLite(), dbname = dbPath)

