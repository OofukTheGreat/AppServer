using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppServer.Models;

public partial class Player
{
    [Key]
    public int PlayerId { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(100)]
    public string? Password { get; set; }

    [StringLength(100)]
    public string? DisplayName { get; set; }

    public byte[]? ProfilePicture { get; set; }

    public bool? IsAdmin { get; set; }

    [InverseProperty("Creator")]
    public virtual ICollection<Level> Levels { get; set; } = new List<Level>();

    [InverseProperty("Player")]
    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();
}
