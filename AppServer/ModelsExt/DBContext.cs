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
        public List<Score>? GetWonScoresByLevel(int levelid)
        {
            return this.Scores.Where(s => s.LevelId == levelid && s.HasWon == true).ToList();
        }
        public Score HighestScoreByLevel(int levelid)
        {
            List<Score> tempscores = GetWonScoresByLevel(levelid);
            tempscores.OrderBy(s => s.Time);
            return tempscores.FirstOrDefault();
        }
        public List<Score>? GetPlayerWinningScores(int playerid)
        {
            List<Score> tempscores = new List<Score>();
            foreach (Level l in Levels)
            {
                tempscores.Add(HighestScoreByLevel(l.LevelId));
            }
            return tempscores.Where(s => s.PlayerId == playerid).ToList();
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
