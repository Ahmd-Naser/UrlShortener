using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Domain.Entities;

public class UrlVisit
{
    public int Id { get; set; } 

    public int UrlId { get; set; }

    public DateTime VisitDate { get; set; } = DateTime.UtcNow;

    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; } // نوع المتصفح والموبايل أو الكمبيوتر

    public Url Url { get; set; } = null!;
}