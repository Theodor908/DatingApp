using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace API.Entities;

public class Group
{
    [Key]
    public required string Name { get; set; }
    public ICollection<Connection> Connections { get; set; } = [];
}
