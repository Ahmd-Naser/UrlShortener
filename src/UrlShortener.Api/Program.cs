using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using UrlShortener.Application.Services;
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

// تفعيل الـ In-Memory Caching في السيرفر
builder.Services.AddMemoryCache();

// تسجيل الـ Pipeline Behavior بتاع MediatR
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UrlShortener.Application.Common.Behaviors.CachingBehavior<,>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();