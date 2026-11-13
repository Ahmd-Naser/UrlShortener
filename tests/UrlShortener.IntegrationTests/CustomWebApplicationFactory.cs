using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace UrlShortener.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer;
    private readonly RedisContainer _redisContainer;

    public CustomWebApplicationFactory()
    {
        // تجهيز كونتينر SQL Server بحد أقصى 2 جيجا رام
        _sqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithCreateParameterModifier(cmd => cmd.HostConfig.Memory = 2L * 1024 * 1024 * 1024)
            .Build();

        // تجهيز كونتينر Redis بحد أقصى 100 ميجا رام
        _redisContainer = new RedisBuilder()
            .WithImage("redis:alpine")
            .WithCreateParameterModifier(cmd => cmd.HostConfig.Memory = 100 * 1024 * 1024)
            .Build();
    }

    // هنا بنتدخل قبل ما الـ API يقوم عشان نغير الإعدادات
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // بنمسح أي إعدادات قديمة ونحط الـ Connection Strings اللي الكونتينرات عملتها
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", _sqlContainer.GetConnectionString() },
                { "ConnectionStrings:Redis", _redisContainer.GetConnectionString() }
            });
        });
    }

    // الدالة دي بتشتغل قبل أي Test ما يبدأ
    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();
        await _redisContainer.StartAsync();
    }

    // الدالة دي بتشتغل بعد ما كل الـ Tests تخلص عشان تنظف وراها
    public async Task DisposeAsync()
    {
        await _sqlContainer.DisposeAsync();
        await _redisContainer.DisposeAsync();
    }
}