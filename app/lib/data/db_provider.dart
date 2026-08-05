import 'dart:async';

import 'package:path/path.dart';
import 'package:sqflite/sqflite.dart';

import '../models/team.dart';

class DbProvider {
  static final DbProvider _instance = DbProvider._internal();
  factory DbProvider() => _instance;
  DbProvider._internal();

  static Database? _database;

  Future<Database> get database async {
    if (_database != null) return _database!;
    _database = await _initDB('football_gm.db');
    return _database!;
  }

  Future<Database> _initDB(String fileName) async {
    final dbPath = await getDatabasesPath();
    final path = join(dbPath, fileName);
    return openDatabase(path, version: 1, onCreate: _onCreate);
  }

  FutureOr<void> _onCreate(Database db, int version) async {
    await db.execute('''
      CREATE TABLE teams(
        id INTEGER PRIMARY KEY,
        name TEXT NOT NULL,
        city TEXT NOT NULL
      )
    ''');
  }

  Future<int> insertTeam(Team team) async {
    final db = await database;
    return db.insert('teams', team.toJson(), conflictAlgorithm: ConflictAlgorithm.replace);
  }

  Future<List<Team>> getTeams() async {
    final db = await database;
    final rows = await db.query('teams');
    return rows.map(Team.fromJson).toList();
  }

  Future<void> close() async {
    final db = await database;
    await db.close();
    _database = null;
  }
}
