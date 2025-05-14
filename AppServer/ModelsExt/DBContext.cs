using System;
using System.Collections.Generic;
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
            return this.Scores.Where(p => p.PlayerId == playerid).ToList();
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
