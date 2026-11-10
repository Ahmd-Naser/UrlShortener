using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using UrlShortener.Infrastructure.Persistence;

namespace UrlShortener.Infrastructure.BackgroundJobs;

public class UrlCleanupService(
    IServiceProvider serviceProvider,
    ILogger<UrlCleanupService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // هنستخدم PeriodicTimer (أحدث وأفضل طريقة في دوت نت للـ Background Tasks)
        // هنخليها تشتغل كل 24 ساعة مثلاً (تقدر تخليها كل ساعة للتجربة)
        using var timer = new PeriodicTimer(TimeSpan.FromHours(24));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                // الـ BackgroundService بيبقى Singleton، والـ DbContext بيبقى Scoped
                // عشان كده لازم نفتح Scope جديد في كل لفة
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // مسح كل الروابط اللي تاريخ انتهائها أقدم من وقتنا الحالي
                var deletedCount = await context.Urls
                    .Where(u => u.ExpirationDate < DateTime.UtcNow)
                    .ExecuteDeleteAsync(stoppingToken);

                if (deletedCount > 0)
                {
                    logger.LogInformation("Background Job: Successfully deleted {Count} expired URLs.", deletedCount);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Background Job: An error occurred while cleaning up expired URLs.");
            }
        }
    }
}