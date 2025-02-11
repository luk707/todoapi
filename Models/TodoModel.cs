using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models;

public class Todo
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public required string Name { get; set; }

    public bool Completed { get; set; }
}
