using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Application.Common.Interfaces;

public interface ICacheableQuery
{
    // المفتاح اللي هنشيل بيه الداتا في الكاش (مثلاً: "url_aB3x9")
    string CacheKey { get; }

    // مدة بقاء الداتا في الكاش (ممكن نسيبها null ونعملها قيمة افتراضية)
    TimeSpan? Expiration { get; }
}