using System.Diagnostics;

// -------------------------------------------------------------
// 1. AŞAMA: Thread.Sleep Kullanımı (Derleyici Uyarısını Görme)
// -------------------------------------------------------------
Console.WriteLine("=== 1. DENEY: Thread.Sleep (Bloklayıcı / Sync) ===");
var sw = Stopwatch.StartNew();

// Sırayla Çağırma
await SahteDbCagrisiThreadSleep("kullanıcı", 1000);
await SahteDbCagrisiThreadSleep("siparişler", 1000);
await SahteDbCagrisiThreadSleep("ürünler", 1000);
Console.WriteLine($"Thread.Sleep Sırayla: {sw.ElapsedMilliseconds} ms");

sw.Restart();

// Task.WhenAll ile Birlikte Çalıştırma Denemesi
await Task.WhenAll(
    SahteDbCagrisiThreadSleep("kullanıcı", 1000),
    SahteDbCagrisiThreadSleep("siparişler", 1000),
    SahteDbCagrisiThreadSleep("ürünler", 1000)
);
Console.WriteLine($"Thread.Sleep Birlikte (WhenAll): {sw.ElapsedMilliseconds} ms");

Console.WriteLine();

// -------------------------------------------------------------
// 2. AŞAMA: Task.Delay Kullanımı (Gerçek Asenkron / Non-Blocking)
// -------------------------------------------------------------
Console.WriteLine("=== 2. DENEY: Task.Delay (Asenkron / Async) ===");
sw.Restart();

// Sırayla Çağırma
await SahteDbCagrisiTaskDelay("kullanıcı", 1000);
await SahteDbCagrisiTaskDelay("siparişler", 1000);
await SahteDbCagrisiTaskDelay("ürünler", 1000);
Console.WriteLine($"Task.Delay Sırayla: {sw.ElapsedMilliseconds} ms");

sw.Restart();

// Task.WhenAll ile Birlikte Çalıştırma
await Task.WhenAll(
    SahteDbCagrisiTaskDelay("kullanıcı", 1000),
    SahteDbCagrisiTaskDelay("siparişler", 1000),
    SahteDbCagrisiTaskDelay("ürünler", 1000)
);
Console.WriteLine($"Task.Delay Birlikte (WhenAll): {sw.ElapsedMilliseconds} ms");


// --- METOT TANIMLARI ---

// 1. Metot: CS1998 uyarısı verecek metot (async var ama await yok, Thread.Sleep var)
static async Task<string> SahteDbCagrisiThreadSleep(string ad, int ms)
{
    Thread.Sleep(ms);
    return ad;
}

// 2. Metot: Gerçek asenkron metot (await Task.Delay var)
static async Task<string> SahteDbCagrisiTaskDelay(string ad, int ms)
{
    await Task.Delay(ms);
    return ad;
}