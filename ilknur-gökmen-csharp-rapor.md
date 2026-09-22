# C# Stajı 1. Gün Raporu

**Tarih:** 21 Eylül 2026  
**Hazırlayan:** İlknur  
**Proje Adı:** `gun1-konsol`  

---

## 1. Adım: Kurulum ve Doğrulama

Terminal üzerinde kurulumlar doğrulanmış ve sistem bilgileri kaydedilmiştir.

| Soru | Cevabınız |
|---|---|
| **.NET SDK sürümü** | 10.0.401 |
| **Kurulu runtime'lar** | Microsoft.NETCore.App 10.0.401 |
| **Git sürümü** | git version 2.43.0.windows.1 |
| **Hangi editörü seçtiniz** | Visual Studio 2022 Community |

> **Düşünün (SDK vs Runtime):** **SDK (Software Development Kit)**, C# kodlarını derlemek, geliştirmek ve paketlemek için gerekli derleyici (`csc`) ve geliştirme araçlarını içerir; geliştirici makinesinde şarttır. **Runtime** ise derlenmiş uygulamanın çalışması için gerekli çalışma zamanı ortamını sağlar; uygulamanın canlıya alınacağı sunucuda sadece Runtime bulunması yeterlidir.

---

## 2. Adım: İlk Proje ve Ne Oluştuğu

### `.csproj` Dosyası İncelemesi
* **`<TargetFramework>`:** Uygulamanın .NET 10.0 hedef platformunda derleneceğini belirtir.
* **`<Nullable>enable</Nullable>`:** Derleyici seviyesinde Nullable Reference Types özelliğini aktif ederek olası `NullReferenceException` risklerinde derleyicinin uyarı vermesini sağlar.
* **`<ImplicitUsings>enable</ImplicitUsings>`:** `System`, `System.Collections.Generic` gibi sık kullanılan ad alanlarını (namespace) otomatik içeri aktarır.

### `bin/` ve `obj/` Klasör İncelemesi
* **`bin/Debug/net10.0/` İçeriği:** `.dll` dosyası uygulamanın platform bağımsız Ara Dil (IL - Intermediate Language) kodlarını barındırır. `.exe` dosyası ise Windows işletim sisteminde bu IL kodunu çalıştıran hafif başlatıcıdır.
* **Git Durumu:** `bin` ve `obj` klasörleri derleme sırasında üretilen geçici (generated) dosyalar olduğu için repoya girmemelidir. `dotnet new gitignore` komutu bu klasörleri otomatik olarak takipten hariç tutmuştur.

> **Düşünün (Top-Level Statements):** C# 9.0 ile gelen "Top-Level Statements" (Üst Düzey Bildirimler) özelliği sayesinde derleyici arka planda `class Program` ve `static void Main(string[] args)` kalıp kodlarını otomatik olarak oluşturur, geliştiricinin doğrudan ana kodu yazmasına olanak tanır.

---

## 3. Adım: Tiplerin Sürprizleri

Aşağıdaki ifadeler çalıştırılmadan önce tahmin edilmiş, ardından gerçek çıktılar kaydedilmiştir:

| Satır / İfade | Tahmininiz | Gerçek Çıktı |
|---|---|---|
| `0.1 + 0.2` | `0.3` | `0.30000000000000004` |
| `0.1 + 0.2 == 0.3` | `True` | `False` |
| `0.1m + 0.2m == 0.3m` | `True` | `True` |
| `buyuk + 1` | `Hata fırlatır` | `-2147483648` |
| `7 / 2` | `3.5` | `3` |
| `7 / 2.0` | `3.5` | `3.5` |

* **Ondalık Ayırıcı:** Kod içerisinde nokta (`.`) yazılmasına rağmen işletim sisteminin Türkçe dil/kültür (Culture) ayarlarından dolayı terminaldeki ondalık ayırıcı **virgül (,)** olarak basılmıştır.
* **`checked` Deneyimi:** `checked(buyuk + 1)` yazıldığında taşma gizlenmeyip **`System.OverflowException`** hatası fırlatılmıştır.

> **Düşünün (Decimal vs Double & Taşma):** `double` ikili (binary) tabanda kayan noktalı hesaplama yaptığı için finansal işlemlerde kuruş farkı/hassasiyet kayması yaratır. `decimal` ise 10'luk tabanda tam hassasiyet sunduğundan tahsilat ekranlarında mutlaka tercih edilmelidir. Kritik finansal veya veri bütünlüğü gerektiren hesaplamalarda taşmanın sessizce negatif sayıya dönmesi yerine `checked` ile hata vermesini isteriz.

---

## 4. Adım: Null ve Derleyicinin Uyarıları

### Derleyici Uyarıları (`dotnet build`)
* **`CS8600`:** Converting null literal or possible null value to non-nullable type.
* **`CS8602`:** Dereference of a possibly null reference.

### `Ctrl+Z` (Girdi Bitti / Null) Hata Senaryosu
Ad sorulduğunda **Ctrl+Z** verilerek girdi kesildiğinde alınan hata:
* **Exception:** `System.NullReferenceException: Object reference not set to an instance of an object.`
* **Konum:** `Program.cs:line 3` (`ad.ToUpper()` satırı)

### Çözüm Yöntemleri Analizi
1. `string?` ve `?.` kullanımı: Hem derleyici uyarısını kaldırır hem de `null` geldiğinde uygulamayı çöktürmez. **(Gerçek Çözüm)**
2. `?? ""` eklemek: `null` gelirse varsayılan boş string atar, çökmesi imkansızlaşır. **(Gerçek Çözüm)**
3. `Console.ReadLine()!` kullanımı: Derleyici uyarısını susturur ancak `Ctrl+Z` basıldığında runtime çökmeye devam eder. **(Sadece Uyarıyı Susturur)**

> **Düşünün (TreatWarningsAsErrors):** Derleyici uyarılarını hataya çevirmek (`TreatWarningsAsErrors=true`), potansiyel runtime hatalarının (örneğin gözden kaçan null erişimleri) derleme aşamasında fark edilmesini sağlar. Kurumun Atis Core projesinde bir alt projede bu ayar unutulduğu için gözden kaçan bir uyarı canlıda ekranların açılmamasına yol açmıştır.

---

## 5. Adım: Konsolda Todo Uygulaması

`List<string>` ve `HashSet<int>` koleksiyon yapıları ile geliştirilen güvenli konsol Todo uygulamasının çalışma oturum çıktısı:

```text
1) Ekle  2) Listele  3) Tamamla  4) Sil  0) Çıkış
Seçim: 1
Başlık: C# SDK Kurulumunu Tamamla
Görev eklendi.

1) Ekle  2) Listele  3) Tamamla  4) Sil  0) Çıkış
Seçim: 1
Başlık: Gitignore Kurallarını İncele
Görev eklendi.

1) Ekle  2) Listele  3) Tamamla  4) Sil  0) Çıkış
Seçim: 1
Başlık: Staj Raporunu Hazırla
Görev eklendi.

1) Ekle  2) Listele  3) Tamamla  4) Sil  0) Çıkış
Seçim: 3
Tamamlanacak görev numarası: 0
0. numaralı görev tamamlandı olarak işaretlendi.

1) Ekle  2) Listele  3) Tamamla  4) Sil  0) Çıkış
Seçim: 2
0. [x] C# SDK Kurulumunu Tamamla
1. [ ] Gitignore Kurallarını İncele
2. [ ] Staj Raporunu Hazırla

1) Ekle  2) Listele  3) Tamamla  4) Sil  0) Çıkış
Seçim: 0
Programdan çıkılıyor... 


```


## 6. Adım: Kırın (Hata Testleri)

Uygulamanın sınır durumları (edge-cases) ve veri doğrulama eksiklerinin oluşturduğu hatalar test edilerek sonuçlar kaydedilmiştir.

#### Test Sonuçları
1. **Ekle'de Boş Enter:** 
   * *Gözlem:* `string.IsNullOrWhiteSpace` kontrolü eklenmeden önce listede anlamsız boş bir metin (`""`) oluştu. Kontrol eklendikten sonra boş görev eklenmesi engellendi.
2. **Tamamla'da "iki" Yazmak:** 
   * *Gözlem:* `int.TryParse` yapısı kullanıldığında program çökmeyip geçersiz numara uyarısı verdi. Kod geçici olarak `int.Parse` işlemine çekildiğinde ise metin tamsayıya çevrilemediği için **`System.FormatException`** hatası fırlatıldı ve uygulama anında çöktü.
3. **Olmayan Numarayı Silme (`99`):** 
   * *Gözlem:* `silIndex < gorevler.Count` sınır kontrolü yapılmadığı senaryoda liste indeks sınırları aşıldığı için **`System.ArgumentOutOfRangeException`** hatası alındı. Sınır kontrolü eklendiğinde uygulama güvenli şekilde uyarı verdi.
4. **İndeks Kayma Testi (A, B, C Senaryosu):**
   * *Uygulanan Adımlar:* `A`, `B` ve `C` görevleri eklendi. `1` numaralı `B` görevi tamamlandı (`[x]`). `0` numaralı `A` görevi silindi ve liste yeniden çağrıldı.
   * *Sonuç:* Ekranda **C görevi tamamlanmış (`[x]`) olarak göründü!** Tamamlanan görev aslında `B` olmasına rağmen `C` tamamlanmış gibi gösterildi.

> **Düşünün (Hatanın Nedeni):** Görev kimliği (ID) olarak nesneye özel sabit bir değer yerine listenin anlık indeks numarası (0, 1, 2) kullanılmıştır. `A` görevi silindiğinde `B` ve `C` görevleri listede yukarı kaymış; ancak `HashSet` içerisinde saklanan `1` indeksi sabit kaldığı için silme işleminden sonra 1. indekse yerleşen `C` görevini hatalı şekilde tamamlanmış olarak işaretlemiştir.