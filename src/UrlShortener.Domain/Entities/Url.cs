
namespace UrlShortener.Domain.Entities;

public class Url
{
    public int Id { get;set; }
    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortenedCode { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ExpirationDate { get; set; }

    public ICollection<UrlVisit> Visits { get; set; } = [];
}
