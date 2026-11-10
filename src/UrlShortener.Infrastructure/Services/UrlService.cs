using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Common.Errors;
using UrlShortener.Application.Common.Models;
using UrlShortener.Application.Services;
using UrlShortener.Application.Url.Commands.AddUrl;
using UrlShortener.Application.Url.Queries.GetOriginalUrl;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Persistence;

namespace UrlShortener.Infrastructure.Services;

public class UrlService(ApplicationDbContext context) : IUrlService
{
    private readonly ApplicationDbContext _context = context;

    private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int ShortCodeLength = 7;
    private const int defaultExpirationDays = 30;

    public async Task<Result<ShortenedUrl>> AddUrl(AddUrlCommand request, CancellationToken cancellationToken)
    {
        string shortCode;

        do
        {
            shortCode = GenerateRandomCode();

        } while(_context.Urls.Any(u => u.ShortenedCode.Equals(shortCode) ) );

        var finalUrl = request.OriginalUrl;

        if(!finalUrl.StartsWith("http://") && !finalUrl.StartsWith("https://"))
        {
            finalUrl = "http://" + finalUrl;
        }

        var url = new Url
        {
            ShortenedCode = shortCode,
            OriginalUrl = finalUrl,
            ExpirationDate = DateTime.UtcNow.AddDays(defaultExpirationDays)
        };

        await _context.Urls.AddAsync(url, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(new ShortenedUrl(shortCode) );
    }


    public async Task<Result<OriginalUrlResponse>> GetOriginalUrl(GetOriginalUrlQuery request, CancellationToken cancellationToken)
    {
        var url = await _context.Urls.FirstOrDefaultAsync(u => u.ShortenedCode.Equals(request.ShortenedUrl), cancellationToken);

        if (url == null)
            return Result.Failure<OriginalUrlResponse>(UrlErrors.NotFound(request.ShortenedUrl));

        return Result.Success( new OriginalUrlResponse(url.OriginalUrl) );
    }
    private string GenerateRandomCode()
    {
        var codeChars = new char[ShortCodeLength];
        for (int i = 0; i < ShortCodeLength; i++)
        {
            codeChars[i] = Alphabet[Random.Shared.Next(Alphabet.Length)];
        }
        return new string(codeChars);
    }
}
