using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.IO;


public class DBHelper: MonoBehaviour
{
    private string dbPath;
    private PlayerStats ps = PlayerStats.Instance;
    private Game game = Game.Instance;

    private void Awake()
    {
        dbPath = Path.Combine(Application.persistentDataPath, "database.db");
        CreateDatabaseAndTable();
        Debug.Log($"Database path: {dbPath}");

    }

    public void CreateDatabaseAndTable()
    {
        using (var db = new SQLiteConnection(dbPath))
        {
            db.CreateTable<PlayerDB>();
        }
    }

    public void AddNewPlayer()
    {
        using (var db = new SQLiteConnection(dbPath))
        {
            var player = new PlayerDB()
            {
                Name = ps.PlayerName,
                HighScore = ps.HighScore,
                CurrentScore = ps.Score,
                Level = game.level?.levelNumber ?? 0,
                Mode = game.gameMode,
                AimType = game.aimType
            };
            db.Insert(player);
            ps.PlayerDBId = player.Id;
        }
    }

    public void SaveCurentPlayer()
    {
        if (ps.PlayerDBId == 0) AddNewPlayer();

        using (var db = new SQLiteConnection(dbPath))
        {
            var player = db.Get<PlayerDB>(ps.PlayerDBId); //db.Find<PlayerDB>(r => r.Name == ps.PlayerName);

            player.HighScore = ps.HighScore;
            player.CurrentScore = ps.Score;
            player.Level = game.level?.levelNumber??0;
            player.Mode = game.gameMode;
            player.AimType = game.aimType;
            db.Update(player);
            
        }
    }

    public List<PlayerDB> GetAllScores()
    {
        using (var db = new SQLiteConnection(dbPath))
        {
            var list = db.Table<PlayerDB>();
            return list.OrderByDescending(r => r.HighScore).ToList();
        }
    }

    //public bool LoadPlayer(string playerName)
    //{
    //    using (var db = new SQLiteConnection(dbPath))
    //    {
    //        var player = db.Find<PlayerDB>(r=>r.Name == playerName);
            
    //        if (player == null) return false;

    //        ps.HighScore = player.HighScore;
    //        ps.Score = player.CurrentScore;
    //        game.level.levelNumber = player.Level;
    //        game.gameMode = player.Mode;
    //        game.aimType = player.AimType;

    //        return true;

    //    }
    //}

    public class PlayerDB
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } 
        public string Name { get; set; }    
        public int HighScore { get; set; }
        public int CurrentScore { get; set; }
        public int Level { get; set; }
        public GameMode Mode { get; set; }
        public AimType AimType { get; set; }
    }



}

