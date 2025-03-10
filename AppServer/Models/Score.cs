using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppServer.Models;

[PrimaryKey("PlayerId", "LevelId", "HasWon")]
public partial class Score
{
    [Key]
    public int PlayerId { get; set; }

    [Key]
    public int LevelId { get; set; }

    public int Time { get; set; }

    [StringLength(2000)]
    public string? CurrentProgress { get; set; }

    [Key]
    public bool HasWon { get; set; }

    [ForeignKey("LevelId")]
    [InverseProperty("Scores")]
    public virtual Level Level { get; set; } = null!;

    [ForeignKey("PlayerId")]
    [InverseProperty("Scores")]
    public virtual Player Player { get; set; } = null!;
}
