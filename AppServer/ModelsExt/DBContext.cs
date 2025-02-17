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
        public List<Score>? GetScores(int levelid)
        {
            return this.Scores.Where(s => s.HasWon == true && s.LevelId == levelid).ToList();
        }
        public List<Player>? GetPlayers()
        {
            return this.Players.ToList();
        }
    }
}
