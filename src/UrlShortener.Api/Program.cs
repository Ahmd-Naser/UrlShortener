using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Threading.RateLimiting;
using UrlShortener.Application.Services;
using UrlShortener.Infrastructure.BackgroundJobs;
using UrlShortener.Infrastructure.Persistence;
using UrlShortener.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUrlService, UrlService>();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(UrlShortener.Application.Url.Commands.AddUrl.AddUrlCommand).Assembly));

builder.Services.AddControllers();

// إعداد Distributed Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    // لو حاططها في appsettings يفضل تجيبها من هناك، بس للتجربة هنكتبها مباشرة
    options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    // الـ InstanceName بيحط Prefix قبل أي مفتاح في Redis عشان لو استخدمت نفس السيرفر لمشاريع تانية ميتداخلوش
    options.InstanceName = "UrlShortener_";
});

// تسجيل الـ Pipeline Behavior بتاع MediatR
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UrlShortener.Application.Common.Behaviors.CachingBehavior<,>));

builder.Services.AddHostedService<UrlCleanupService>();

builder.Services.AddRateLimiter(options =>
{
    // لو اليوزر اتخطى الحد المسموح بيه، هنرجعله كود 429 Too Many Requests
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // السياسة الأولى: عامة للسيستم كله (100 ريكويست في الدقيقة)
    options.AddFixedWindowLimiter("GlobalPolicy", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0; // مفيش طابور انتظار، اللي هيتخطى هيترفض فوراً
    });

    // السياسة التانية: صارمة شوية للـ POST Endpoint عشان نمنع إنشاء روابط وهمية كتير
    options.AddFixedWindowLimiter("CreateUrlPolicy", opt =>
    {
        opt.PermitLimit = 5; // 5 روابط بس
        opt.Window = TimeSpan.FromMinutes(1); // كل دقيقة
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
});

builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "SQL Server")
    .AddRedis(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379", name: "Redis Cache");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

}

// تطبيق المايجريشنز أوتوماتيك عند تشغيل السيرفر
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // ده بيكريت الداتابيز والجداول لو مش موجودين
    context.Database.Migrate();
}

app.UseRateLimiter();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapControllers();

app.Run();