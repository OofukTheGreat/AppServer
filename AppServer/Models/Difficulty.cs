using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppServer.Models;

public partial class Difficulty
{
    [Key]
    public int DifficultyId { get; set; }

    [StringLength(100)]
    public string? DifficultyName { get; set; }

    [InverseProperty("Difficulty")]
    public virtual ICollection<Level> Levels { get; set; } = new List<Level>();
}
