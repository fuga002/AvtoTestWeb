using System.ComponentModel.DataAnnotations;

namespace AvtoTest.Data.Entities;

public class Result
{
    [Key]
    public int Id { get; set; }

    public byte TicketId { get; set; }


    public byte CorrectAnswersCount { get; set; }

    public byte InCorrectAnswersCount =>(byte) (TotalAnswersCount - CorrectAnswersCount);

    public const byte TotalAnswersCount = 20;

    public string UserId { get; set; }
    public CustomUser? CustomUser { get; set; }

    public DateTime  CreatedAt { get; set; } = DateTime.Now;
}