using System.ComponentModel.DataAnnotations;

namespace AvtoTest.Data.Entities;


public class CustomUser
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<Result>? Results { get; set; }
}