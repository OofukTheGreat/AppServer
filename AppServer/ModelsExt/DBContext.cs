using System;
using System.Collections.Generic;
using AppServer.DTO;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
namespace AppServer.Models
{
    public partial class DBContext : DbContext
    {
        public Player? GetUser(string Email)
        {
            return this.Players.Where(u => u.Email == Email).FirstOrDefault();
        }
        public List<Score>? GetScoresByLevel(int levelid)
        {
            return this.Scores.Where(s => s.HasWon == true && s.LevelId == levelid).ToList();
        }
        public List<Score>? GetScoresByPlayer(int playerid)
        {
            return this.Scores.Where(s => s.PlayerId == playerid).ToList();
        }
        public List<Score>? GetPlayerWinningScores(int playerid)
        {
            List<Score> tempscores = new List<Score>();
            List<Level> templevels = new List<Level>(Levels);
            foreach (Level l in templevels)
            {
                List<Score> unsorted = this.Scores.Where(s => s.LevelId == l.LevelId && s.HasWon == true).ToList();
                unsorted.OrderBy(s => s.Time);
                tempscores.Add(unsorted.First());
            }
            return tempscores.Where(s => s.PlayerId == playerid).ToList();
        }
        public List<Player>? GetPendingLevelMakers()
        {
            List<Level> pendinglevels = this.Levels.Where(l => l.StatusId == 1).ToList();
            List<Player> tempplayers = new List<Player>(Players);
            List<Player> makers = new List<Player>();
            foreach (Player p in tempplayers)
            {
                if (pendinglevels.Where(l => l.CreatorId == p.PlayerId).FirstOrDefault() != null) makers.Add(p);
            }
            return makers;
        }
        public List<Player>? GetPlayers()
        {
            return this.Players.ToList();
        }
        public List<Level>? GetApprovedLevels()
        {
            return this.Levels.Where(L => L.StatusId == 2).ToList();
        }
        public List<Level>? GetPendingLevels()
        {
            return this.Levels.Where(L => L.StatusId == 1).ToList();
        }
    }
}
