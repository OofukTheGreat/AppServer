using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AppServer.Models;

public partial class DBContext : DbContext
{
    public DBContext()
    {
    }

    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }
    public virtual DbSet<Difficulty> Difficulties { get; set; }

    public virtual DbSet<Level> Levels { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Score> Scores { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server = (localdb)\\MSSQLLocalDB;Initial Catalog=AppServer_DB;User ID=AdminLogin;Password=1234;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Difficulty>(entity =>
        {
            entity.HasKey(e => e.DifficultyId).HasName("PK__Difficul__161A3267F2BF8874");
        });

        modelBuilder.Entity<Level>(entity =>
        {
            entity.HasKey(e => e.LevelId).HasName("PK__Levels__09F03C26739D9270");

            entity.HasOne(d => d.Creator).WithMany(p => p.Levels).HasConstraintName("FK__Levels__CreatorI__2A4B4B5E");

            entity.HasOne(d => d.Difficulty).WithMany(p => p.Levels).HasConstraintName("FK__Levels__Difficul__2C3393D0");

            entity.HasOne(d => d.Status).WithMany(p => p.Levels).HasConstraintName("FK__Levels__StatusId__2B3F6F97");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.PlayerId).HasName("PK__Players__4A4E74C89B16E3B2");
        });

        modelBuilder.Entity<Score>(entity =>
        {
            entity.HasOne(d => d.Level).WithMany(p => p.Scores)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Scores__LevelId__300424B4");

            entity.HasOne(d => d.Player).WithMany(p => p.Scores)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Scores__PlayerId__2F10007B");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Statuses__C8EE206355BC21A2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
