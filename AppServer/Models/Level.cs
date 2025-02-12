using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppServer.Models;

public partial class Level
{
    [Key]
    public int LevelId { get; set; }

    [StringLength(100)]
    public string? Title { get; set; }

    [StringLength(2000)]
    public string? Layout { get; set; }

    public int? CreatorId { get; set; }

    public int? StatusId { get; set; }

    public int? Size { get; set; }

    [ForeignKey("CreatorId")]
    [InverseProperty("Levels")]
    public virtual Player? Creator { get; set; }

    [InverseProperty("Level")]
    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();

    [ForeignKey("StatusId")]
    [InverseProperty("Levels")]
    public virtual Status? Status { get; set; }
}
