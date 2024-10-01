using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppServer.Models;

public partial class Status
{
    [Key]
    public int StatusId { get; set; }

    [StringLength(100)]
    public string? StatusName { get; set; }

    [InverseProperty("Status")]
    public virtual ICollection<Level> Levels { get; set; } = new List<Level>();
}
