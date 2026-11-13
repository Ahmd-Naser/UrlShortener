using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace UrlShortener.IntegrationTests;

public class UrlsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UrlsControllerTests(CustomWebApplicationFactory factory)
    {
        // بنكريت Client وهمي بيكلم الـ API اللي شغال في الميموري
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateUrl_WhenValidOriginalUrl_ShouldReturnOkAndSaveToDatabase()
    {
        // Arrange: بنجهز الـ Request اللي هيتبعت للـ Endpoint
        // استخدمت اسمك كداتا حقيقية عشان التيست يكون واقعي
        var request = new
        {
            OriginalUrl = "https://github.com/AhmedNasser/UrlShortener"
        };

        // Act: بننفذ الـ POST Request زي ما Postman بيعمل بالظبط
        var response = await _client.PostAsJsonAsync("/api/urls", request);

        // Assert: بنختبر النتيجة

        // 1. نتأكد إن الـ Status Code هو 200 OK
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // 2. نقرأ الرد ونتأكد إنه مش فاضي
        var responseText = await response.Content.ReadAsStringAsync();
        responseText.Should().NotBeNullOrEmpty();

        // 3. نتأكد إن الرد بيحتوي على رابط أو الـ ShortCode 
        // (ده بيعتمد على شكل الـ DTO اللي الـ Controller بتاعك بيرجعه)

    }

    //[Fact]
    //public async Task GetOriginalUrl_AfterCreation_ShouldReturnSuccessfully_AndCacheInRedis()
    //{
    //    // 1. Arrange: إنشاء رابط جديد الأول عشان نقدر نطلبه
    //    // حطيتلك بروفايل الـ LeetCode بتاعك كداتا واقعية
    //    var originalUrl = "https://leetcode.com/Ahmed-Naser";
    //    var createResponse = await _client.PostAsJsonAsync("/api/urls", new { OriginalUrl = originalUrl });

    //    // قراءة الرد عشان نطلع الـ ShortCode 
    //    // (ملحوظة: لو الكلاسات عندك أسمائها مختلفة، استبدل UrlResponseDto باسم الكلاس بتاعك)
    //    var responseString = await createResponse.Content.ReadAsStringAsync();
    //    using var document = System.Text.Json.JsonDocument.Parse(responseString);

    //    // بنفترض إن النتيجة راجعة جوه خاصية اسمها Value وجواها ShortCode

    //    var shortCode = document.RootElement.GetProperty("ShortenedUrl").GetProperty("Url").GetString();

    //    Console.WriteLine($"+++++++++++++++||||||||||||||||||||{responseString}");

    //    // 2. Act: الطلب الأول (هيجيب الداتا من SQL Server ويكيشها في Redis)
    //    var firstGet = await _client.GetAsync($"/api/urls/{shortCode}");

    //    // 3. Act: الطلب التاني (هيجيب الداتا من Redis مباشرة بسرعة الصاروخ)
    //    var secondGet = await _client.GetAsync($"/api/urls/{shortCode}");

    //    // 4. Assert: نتأكد إن الطلبين رجعوا نجاح
    //    firstGet.IsSuccessStatusCode.Should().BeTrue();
    //    secondGet.IsSuccessStatusCode.Should().BeTrue();

    //    // نتأكد إن الـ Redirect أو الرد اللي راجع بيحتوي على الرابط الأصلي
    //    var content = await secondGet.Content.ReadAsStringAsync();
    //    content.Should().Contain("leetcode.com");
    //}
}