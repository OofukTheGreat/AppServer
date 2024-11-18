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
    }
}
