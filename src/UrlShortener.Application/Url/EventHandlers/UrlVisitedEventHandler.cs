using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Services;
using UrlShortener.Application.Url.Dtos;
using UrlShortener.Application.Url.Events;

namespace UrlShortener.Application.Url.EventHandlers;

public class UrlVisitedEventHandler(
    IUrlService urlService,
    ILogger<UrlVisitedEventHandler> logger) : INotificationHandler<UrlVisitedEvent>
{
    public async Task Handle(UrlVisitedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var visitDto = new VisitDetailsDto(
                notification.ShortenedCode,
                notification.IpAddress,
                notification.UserAgent);

            await urlService.RecordVisitAsync(visitDto, cancellationToken);

            logger.LogInformation("Analytics: Visit recorded for {ShortCode}", notification.ShortenedCode);
        }
        catch (Exception ex)
        {
            // مهم جداً: بنمسك الـ Exception هنا عشان لو الداتابيز وقعت، 
            // عملية الـ Redirect الأساسية بتاعت اليوزر متضربش ويكمل رحلته عادي!
            logger.LogError(ex, "Analytics: Failed to record visit for {ShortCode}", notification.ShortenedCode);
        }
    }
}