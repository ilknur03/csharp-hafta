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


---

## İkinci Gün: Nesneler ve Interface

### 7. Adım: TodoItem Sınıfı (Encapsulation ve Veri Bütünlüğü)

Kapsülleme (Encapsulation) kurallarına uygun olarak `TodoItem.cs` sınıfı oluşturulmuş ve davranışları test edilmiştir.

#### Test Denemeleri ve Sonuçları

1. **Boş Başlık Denemesi (`var t = new TodoItem(1, "");`):**
   * **Sonuç:** Yapıcı metot (constructor) içindeki doğrulama çalıştı ve runtime aşamasında **`System.ArgumentException`** fırlatıldı (*"Başlık boş olamaz. (Parameter 'title')"*).
2. **Doğrudan `IsDone` Değiştirme Denemesi (`t.IsDone = true;`):**
   * **Sonuç:** `IsDone` özelliğinin ayarlayıcısı `private set` olarak kapsüllendiği için program derlenmedi. Derleyici **`CS0272`** hatası verdi (*"Ayarlama erişimcisine erişilemediğinden 'TodoItem.IsDone' özelliği bu bağlamda kullanılamaz"*).

#### Soru & Cevaplar

* **`IsDone`'ın setter'ı `private`, ancak `Complete()` metodu `public`. Neden böyle?**
  * Dışarıdan bilinçsizce `t.IsDone = true` veya `false` şeklinde müdahale edilmesini engellemek için. Görevin tamamlanma durumu nesnenin kendi iş mantığını çalıştıran `Complete()` metodu üzerinden kapsüllenerek yönetilir.

> **Düşünün ("Başlık boş olamaz" Kuralının Yeri):** "Başlık boş olamaz" gibi iş kuralları konsol arayüzünde (`Program.cs`) değil, doğrudan `TodoItem` sınıfının constructor'ında yer almalıdır. Çünkü bu sınıf bugün konsol uygulamasında, yarın Web API'de veya mobil uygulamada kullanılabilir. Kurallar varlık (entity) içinde tutulduğunda, sınıf nereden çağrılırsa çağrılsın hatalı nesne oluşturulması engellenmiş olur.


---

### 8. Adım: Interface (ITodoRepository ve InMemory Implementation)

Esnek, katmanlı bir mimari kurmak amacıyla `ITodoRepository` arayüzü ve bellek üzerinde çalışan `InMemoryTodoRepository` sınıfı projeye dahil edilmiştir.

#### Test Denemesi ve Sonuçlar

1. **Günün 6. Adımındaki İndeks Kayma Testinin Tekrarı:**
   * **Adımlar:** `A`, `B` ve `C` görevleri eklendi[cite: 8, 11]. `2` ID'li `B` görevi tamamlandı (`[x]`)[cite: 8, 11]. `1` ID'li `A` görevi silindi[cite: 8, 11].
   * **Sonuç:** Liste yenilendiğinde `B` görevi kendi benzersiz ID'sini (`ID: 2`) ve tamamlanmış durumunu (`[x]`) korudu; `C` görevi etkilenmedi[cite: 11].

#### Soru & Cevaplar

* **Hata Düzeldi mi? Neden?**
  * **Evet, düzeldi.** Dünkü mimaride görev kimliği olarak listenin anlık sıra numarası (indeksi) kullanılıyordu; bir eleman silindiğinde tüm indeksler kayıyordu[cite: 3]. Yeni yapıda ise her görev nesnesi oluşurken benzersiz (unique) sabit bir `Id` değeri alır. Elemanlar silinse dahi nesnelerin ID'si değişmediği için tamamlanma durumu doğrudan doğru nesneye bağlı kalır[cite: 8, 11].

> **Kurumdaki Karşılığı (AtisCore Eşleşmesi):** İleride çalışılacak `AtisCore` projesinde bu interface `DataAccess/Abstract` klasörü altında `ITodoDal` adıyla bulunur; veritabanına yazan somut sınıf ise `EfTodoDal` olarak adlandırılır[cite: 8]. Adlar değişse de mimari fikir ve katman ayrımı birebir aynıdır[cite: 8].


---

### 9. Adım: İkinci Bir Implementasyon (JsonFileTodoRepository)

Verilerin uygulama kapandığında kaybolmaması için veriyi `todos.json` dosyasında saklayan `JsonFileTodoRepository` sınıfı yazılmış ve sisteme entegre edilmiştir[cite: 8].

#### Ölçüm ve Test Sonuçları

1. **`git diff --stat Program.cs` Ölçümü:**
   * **Değişen Satır Sayısı:** `Program.cs` dosyasında yalnızca 1 satırlık değişiklik yapılmıştır (`InMemoryTodoRepository` yerine `JsonFileTodoRepository` geçirilmiştir)[cite: 8].
2. **Kalıcılık ve Seri Hale Getirme (Serialization) Testi:**
   * Bir görev eklenip tamamlandıktan sonra `todos.json` kontrol edilmiş, dosyada `"IsDone": true` yazdığı görülmüştür[cite: 13].
   * Program kapatılıp açıldığında ekranda görevin `[ ]` (false) geldiği fark edilmiştir[cite: 12].

#### Soru & Cevaplar

* **Program açıldığında tamamlanan görevler neden `false` geldi?**
  * `TodoItem` sınıfında `IsDone` alanı kapsülleme kuralı gereği `private set` yapılmıştı[cite: 8]. `JsonSerializer`, dosyadan okuma yaparken `private` olan ayarlayıcılara erişemediği için alan varsayılan değeri olan `false` ile yüklendi[cite: 8, 12, 13].
* **Çözüm Nasıl Sağlandı?**
  * `IsDone` özelliğinin üstüne `[JsonInclude]` özniteliği eklenerek JSON dönüştürücüsünün `private set` olsa dahi veriyi nesneye aktarması sağlandı[cite: 8].

> **Dependency Inversion Kazanımı:** `Program.cs` dosyasındaki uygulama mantığına dokunmadan, sadece tek bir satır değiştirilerek uygulamanın veri saklama ortamı bellekten fiziki bir JSON dosyasına aktarılmıştır[cite: 8].


---

### 10. Adım: Record ve Class (Değer ve Referans Karşılaştırması)

`class` ile `record` yapıları arasındaki eşitlik mantığı ve metinsel temsil (`ToString`) farkları kodlanarak test edilmiştir[cite: 4].

#### Test Karşılaştırma Tablosu

| İfade / Satır | Tahmininiz | Gerçek Çıktı | Açıklama |
|---|---|---|---|
| `c1 == c2` | `False` | `False` | `class` referans tiptir; bellekteki adresleri kıyaslar[cite: 4]. |
| `r1 == r2` | `True` | `True` | `record` değer bazlı (`value-based`) eşitlik sunar; içerikteki verileri kıyaslar[cite: 4]. |
| `c1` | `NoktaClass` | `NoktaClass` | `class` varsayılan `ToString()` ile sınıf adını basar[cite: 4]. |
| `r1` | `NoktaRecord { X = 1, Y = 2 }` | `NoktaRecord { X = 1, Y = 2 }` | `record` okunabilir, detaylı metinsel temsil üretir[cite: 4]. |

#### Soru & Cevaplar

* **DTO (Data Transfer Object) Nedir?**
  * Katmanlar veya servisler arasında veri taşımak amacıyla kullanılan, içerisinde iş mantığı (business logic) barındırmayan hafif veri taşıyıcı nesnelerdir.
* **Eşitliğin Değere Göre Çalışması Bir DTO İçin Neden Doğaldır?**
  * DTO'ların temel amacı veriyi iletmektir. İki farklı DTO nesnesinin bellekteki adresleri farklı olsa dahi, taşıdıkları alanlar (özellikler) birebir aynıysa iş mantığı açısından eşittirler. `record` yapıları bu değer bazlı eşitliği varsayılan olarak sunduğu için DTO'lar için en uygun yapıdır[cite: 4].
	* 

	---

### 11. Adım: Kırın (2. Gün Hata Testleri)

Uygulamanın nesne yönelimli ve mimari yapısının sınır durumlarındaki davranışları test edilerek sonuçlar kaydedilmiştir[cite: 4].

#### Test Sonuçları ve Gözlemler

1. **Interface Değişikliği Testi (`int Count();` eklenmesi):**
   * **Sonuç:** `dotnet build` komutu sonrası 2 derleme hatası (`CS0535`) alındı[cite: 3]. Hatayı veren dosyalar: `InMemoryTodoRepository.cs` ve `JsonFileTodoRepository.cs`[cite: 3].
   * **Neden:** Sözleşmeye (`interface`) eklenen her yeni imza, o arayüzü uygulayan tüm somut sınıflarda tanımlanmak zorundadır[cite: 3].

2. **Bozuk JSON Testi (Sözdizimi hatası):**
   * **Sonuç:** Menüden listeleme (`GetAll()`) çağrıldığı an **`System.Text.Json.JsonException`** fırlatıldı ve program durdu[cite: 3, 8].
   * **Neden:** JSON ayrıştırıcı biçimsel olarak hatalı bir sözdizimini nesneye dönüştüremedi[cite: 3].

3. **JSON İçinde Boş Başlık Testi (`"Title": ""`):**
   * **Sonuç:** `JsonSerializer.Deserialize` işlemi sırasında 7. adımda `TodoItem` constructor'ına koyduğumuz **`System.ArgumentException`** (*"Başlık boş olamaz."*) fırlatıldı[cite: 8, 12].
   * **Neden:** Veri dosyadan okunsaydı dahi nesne yapıcı metodu (`ctor`) tetiklendiği için kapsülleme ve veri doğrulama kuralları ihlal edilemedi[cite: 8, 12].

4. **Silinen Dosya Testi (`todos.json` silinmesi):**
   * **Sonuç:** Program çökmedi, sorunsuz açıldı ve arka planda otomatik olarak yeni ve boş bir `todos.json` dosyası oluşturuldu[cite: 4, 8].
   * **Neden:** `JsonFileTodoRepository` constructor'ı içindeki `File.Exists` kontrolü uygulamanın çökmesini engelledi[cite: 4, 8].

> **Düşünün (Arayüz Esnekliği):** Birinci maddede görüldüğü üzere bir arayüzü değiştirmek, onu uygulayan tüm sınıfları güncelleme zorunluluğu getirir[cite: 3]. Bu durum, arayüzlerin (interface) olabildiğince küçük, odaklı ve amacına uygun (Interface Segregation) tutulmasının temel sebebidir[cite: 3].


---

## Üçüncü Gün: Veriyle Çalışmak (LINQ ve Async)

### 12. Adım: Aynı Veriyi Üretin

Rastgele veri üreticisine sabit bir tohum (`new Random(42)`) verilerek 10.000 adet ürün nesnesi oluşturulmuş ve `urunler.json` dosyasına kaydedilmiştir.

#### Dosya ve Veri Kontrol Sonuçları

* **`urunler.json` Dosya Boyutu:** 880 KB
* **1. Ürün:** Kategori: Temizlik | Fiyat: 140,91 TL
* **2. Ürün:** Kategori: Gıda | Fiyat: 168,43 TL
* **3. Ürün:** Kategori: Temizlik | Fiyat: 512,92 TL[cite: 13]

> **Düşünün / Not:** `new Random(42)` sabit seed değeri, deterministik rastgele sayı üretimi sağlar[cite: 13]. Bu sayede kod sırası değişmediği sürece her bilgisayarda birebir aynı 10.000 ürün verisi elde edilir[cite: 13].


---

### 13. Adım: Sorgular (LINQ Analizi)

`urunler.json` dosyasındaki 10.000 veri üzerinde LINQ sorguları çalıştırılarak analiz sonuçları elde edilmiş ve arkadaşımın sonuçlarıyla karşılaştırılmıştır[cite: 9, 10, 11].

#### LINQ Sorgu Karşılaştırma Tablosu

| # | Soru | Sizin Cevabınız | Arkadaşınızın Cevabı | Sonuç |
|---|---|---|---|---|
| **1** | Kaç ürünün stoğu 0? | 195 | 195 | **Aynı** |
| **2** | Her kategoride kaç ürün var? | Temizlik: 2053<br>Gıda: 2029<br>Oyuncak: 1987<br>Kırtasiye: 1971<br>Elektronik: 1960 | Temizlik: 2053<br>Gıda: 2029<br>Oyuncak: 1987<br>Kırtasiye: 1971<br>Elektronik: 1960 | **Aynı** |
| **3** | Her kategorinin ortalama fiyatı (TL)? | Oyuncak: 509,44 TL<br>Kırtasiye: 505,65 TL<br>Temizlik: 500,29 TL<br>Elektronik: 499,09 TL<br>Gıda: 494,43 TL | Oyuncak: 509,44 TL<br>Kırtasiye: 505,65 TL<br>Temizlik: 500,29 TL<br>Elektronik: 499,09 TL<br>Gıda: 494,43 TL | **Aynı** |
| **4** | En pahalı beş ürünün Id'leri? | 5003, 6951, 6957, 4302, 448 | 5003, 6951, 6957, 4302, 448 | **Aynı** |
| **5** | Fiyatı 500 ile 600 TL arasında olan Elektronik ürün sayısı? | 194 | 194 | **Aynı** |
| **6** | Toplam stok değeri en yüksek kategori? | Temizlik (25.558.657,24 TL) | Temizlik (25.558.657,24 TL) | **Aynı** |

#### LINQ Kodları

1. `var s1 = urunler.Count(u => u.Stock == 0);`[cite: 10]
2. `var s2 = urunler.GroupBy(u => u.Category).Select(g => new { Kategori = g.Key, Adet = g.Count() });`[cite: 10]
3. `var s3 = urunler.GroupBy(u => u.Category).Select(g => new { Kategori = g.Key, OrtalamaFiyat = Math.Round(g.Average(u => u.Price), 2) });`[cite: 10]
4. `var s4 = urunler.OrderByDescending(u => u.Price).Take(5).Select(u => u.Id);`[cite: 10]
5. `var s5 = urunler.Count(u => u.Category == "Elektronik" && u.Price >= 500m && u.Price <= 600m);`[cite: 10]
6. `var s6 = urunler.GroupBy(u => u.Category).Select(g => new { Kategori = g.Key, ToplamDeger = g.Sum(u => u.Price * u.Stock) }).OrderByDescending(x => x.ToplamDeger).First();`[cite: 10]

---

### 14. Adım: LINQ Ne Zaman Çalışır (Ertelenmiş Çalışma - Deferred Execution)

LINQ sorgularının ertelenmiş çalışma (`Deferred Execution`) ve anında çalışma (`Immediate Execution`) mekanizmaları test edilmiştir[cite: 9].

#### Test Sonuçları ve Tahmin Karşılaştırması

| Test / İfade | Tahmininiz | Gerçek Çıktı |
|---|---|---|
| `buyukler` (`Where`) | `2, 3, 10` | `2, 3, 10` |
| `buyuklerListe` (`ToList`) | `2, 3` | `2, 3, 10` |
| `sayac` Değeri (Filtre çalışma sayısı) | `10001` | `10023` |

#### Soru & Cevaplar

* **`buyukler` ve `buyuklerListe` Neden Farklı Davrandı?**
  * `buyukler` sorgusu `Where` ile tanımlandığında ertelenmiş olarak çalışır; `string.Join` ile tüketildiği an yürütüldüğü için `10` değerini içerir[cite: 9].
  * `buyuklerListe` tanımlandığında eklenen `10` değerini kapsayacak şekilde `.ToList()` çağrıldığı için sonuç belleğe `2, 3, 10` olarak sabitlenmiştir[cite: 9]. Daha sonra eklenen `20` değeri ise listeye yansımamıştır[cite: 9].

* **Sayaç Neden Tam Olarak 10.023 Kez Çalıştı?**
  * `sorgu.Count()` çağrıldığında stoğu 0 olan tüm elemanları bulmak için 10.000 ürünün tamamı taranmıştır (sayaç = 10.000)[cite: 9].
  * `sorgu.First()` tekrar çağrıldığında LINQ sorguyu sıfırdan yeniden çalıştırmış ve ilk stoğu 0 olan ürüne (23. sıradaki ürün) ulaşana kadar 23 eleman daha kontrol etmiştir (sayaç + 23)[cite: 9]. Toplamda sayaç `10023` değerine ulaşmıştır.

---

### 15. Adım: Ölçüm (Liste mi Sözlük mü?)

1.000 adet ürün arama işlemi için `List.FirstOrDefault` ($O(N)$) ile `Dictionary.TryGetValue` ($O(1)$) yapıları `Debug` ve `Release` modlarında test edilerek hız ölçümleri kaydedilmiştir.

#### Hız Ölçüm Tablosu

| Çalıştırma | Mod | List (ms) | Dictionary (ms) |
|---|---|---|---|
| **1** | Debug | 22,70 ms | 0,05 ms |
| **2** | Debug | 20,45 ms | 0,04 ms |
| **3** | Debug | 29,94 ms | 0,05 ms |
| **1** | Release | 21,60 ms | 0,05 ms |
| **2** | Release | 22,29 ms | 0,06 ms |
| **3** | Release | 17,55 ms | 0,05 ms |

#### Soru & Cevaplar

* **Dictionary Kaç Kat Hızlı?**
  * `Dictionary`, ortalama **0,05 ms** arama süresi ile ortalama **22 ms** süren `List` yapısına kıyasla yaklaşık **440 kat** daha hızlı sonuç vermiştir.
* **Ürün Sayısı 10.000 Yerine 1.000.000 Yapılsaydı Fark Nasıl Değişirdi?**
  * `List` aramasının zaman karmaşıklığı $O(N)$ olduğu için eleman sayısı 100 katına çıktığında arama süresi de yaklaşık **100 kat uzardı** (ms düzeyinden saniyeler seviyesine çıkardı).
  * `Dictionary` aramasının zaman karmaşıklığı $O(1)$ olduğu için eleman sayısı 1 milyon da olsa arama süresi **neredeyse hiç değişmez**, sabit hızını korurdu. Bu durum aradaki performans farkının yüz binlerce kata çıkmasına neden olurdu.
* **Bu Ölçümler Neden İki Bilgisayarda/Kişide Farklı Çıkar?**
  * 13. adımdaki matematiksel LINQ analizleri veri bazlı olduğu için her bilgisayarda birebir aynı sonucu verir[cite: 6]. Ancak 15. adımdaki hız ölçümleri bilgisayarların donanımına (CPU hızı, bellek bant genişliği, arka planda çalışan işlem yükü) doğrudan bağlı olduğu için her ortamda farklı milisaniye değerleri üretir[cite: 6].
		

		---

### 16. Adım: Async ve Await (Asenkron Performans Ölçümü)

Asenkron işlemler (`Task.Delay`) ile senkron bloklayıcı işlemler (`Thread.Sleep`) arasındaki çalışma süreleri ve `Task.WhenAll` performansları test edilmiştir.

#### Ölçüm Tablosu

| Deneme | Tahmininiz (ms) | Ölçülen (ms) |
|---|---|---|
| **Task.Delay, sırayla** | 3000 ms | 3063 ms |
| **Task.Delay, birlikte (`WhenAll`)** | 1000 ms | 1013 ms |
| **Thread.Sleep, sırayla** | 3000 ms | 3035 ms |
| **Thread.Sleep, birlikte (`Task.Run` / `WhenAll`)** | 1000 ms | 3035 ms |

#### Soru & Cevaplar

* **`Thread.Sleep` Kullanıldığında `WhenAll` Neden İşe Yaramadı?**
  * `Thread.Sleep`, o an çalışan thread'i tamamen kilitler (blocking) ve geriye asenkron bir görev bırakmaz. Bu nedenle `Task.WhenAll` çağrılsa bile işlemler sırayla yürütülür ve süre düşmez.

* **`Task.Delay` ile `Thread.Sleep` Arasındaki Fark Nedir?**
  * `Thread.Sleep` thread'i dondurur ve bloklar (maliyetli)[cite: 6].
  * `Task.Delay` ise thread'i serbest bırakır (non-blocking)[cite: 6]. Süre dolana kadar thread başka işleri işleyebilir[cite: 6].

* **Bir Web Sunucusunda Thread Bloklamak Neden Kötüdür?**
  * Sunucudaki kısıtlı thread havuzu (Thread Pool) gereksiz yere kilitlenir[cite: 6]. Yoğun trafikte yeni gelen kullanıcılara yanıt verecek thread kalmaz ve sistem kilitlenir (Thread Pool Starvation)[cite: 6].

* **Üçüncü Çağrı İlk İkisinin Sonucuna İhtiyaç Duysaydı `WhenAll` Kullanılabilir miydi?**
  * Hayır[cite: 6]. Veri bağımlılığı olan durumlarda ilk iki işlem `Task.WhenAll` ile paralel bitirilmeli, ardından elde edilen verilerle üçüncü işlem sırayla `await` edilerek çağrılmalıdır[cite: 6].



  ---

## Dördüncü Gün: Web API

### 17. Adım: API Projesi

`dotnet new webapi -n gun4-5-api` komutu ile yeni bir ASP.NET Core Web API projesi oluşturulmuş, `dotnet run` ile çalıştırılmış ve `.http` dosyası üzerinden ilk istek atılmıştır.

#### Program.cs Satır Analizi

* **`builder` (WebApplicationBuilder):** Uygulama sunucusunu ve konfigürasyonlarını kuran ana yapılandırıcıdır[cite: 9].
* **`builder.Services`:** Uygulamada kullanılacak bağımlılıkların (Dependency Injection) sisteme tanıtıldığı Servis Konteyneridir[cite: 9].
* **`app = builder.Build()`:** Servis kayıtlarını dondurup API uygulamasını ayağa kaldıran dönüm noktasıdır[cite: 9].
* **`app.Use...`:** HTTP isteklerinin izleyeceği ara yazılım (Middleware) zincirini belirler[cite: 9].

> **Düşünün (Build Öncesi ve Sonrası):** `builder.Build()` satırından önce servise *neye sahip olması gerektiği* (servis kayıtları) söylenir[cite: 9]. `Build()` edildikten sonra konteyner kilitlenir ve `app.Use...` satırlarıyla *gelen istekleri nasıl işleyeceği* (HTTP Boru Hattı) kurgulanır[cite: 9].

#### WeatherForecast HTTP İstek Çıktısı

* **HTTP Durum Kodu:** `200 OK`[cite: 9]
* **Dönen JSON Yanıtı:**
```json
[
  {
    "date": "2026-09-25",
    "temperatureC": -4,
    "summary": "Bracing",
    "temperatureF": 25
  },
  {
    "date": "2026-09-26",
    "temperatureC": 3,
    "summary": "Bracing",
    "temperatureF": 37
  },
  {
    "date": "2026-09-27",
    "temperatureC": 47,
    "summary": "Hot",
    "temperatureF": 116
  },
  {
    "date": "2026-09-28",
    "temperatureC": 33,
    "summary": "Hot",
    "temperatureF": 91
  },
  {
    "date": "2026-09-29",
    "temperatureC": 52,
    "summary": "Chilly",
    "temperatureF": 125
  }
]

---

### 18. Adım: TodosController (API Endpoints)

2. günde oluşturulan `ITodoRepository` yapısı ASP.NET Core Web API projesine aktarılmış ve RESTful HTTP endpoint'leri dış dünyaya açılmıştır.

#### İstek ve Durum Kodu Tablosu

| İstek | Beklenen Kod | Dönen Kod |
|---|---|---|
| **GET /api/todos** | `200 OK` | `200 OK` |
| **POST /api/todos** | `201 Created` | `201 Created` |
| **GET /api/todos/1** | `200 OK` | `200 OK` |
| **GET /api/todos/999** | `404 Not Found` | `404 Not Found` |
| **PUT /api/todos/1/complete** | `204 No Content` | `204 No Content` |
| **DELETE /api/todos/1** | `204 No Content` | `204 No Content` |
| **DELETE /api/todos/1 (ikinci kez)** | `404 Not Found` | `404 Not Found` |

#### Soru & Cevaplar

* **POST İsteği Neden 200 Yerine 201 Döner? `Location` Başlığı Nedir?**
  * REST standartlarına göre yeni bir kaynak oluşturulduğunda `201 Created` dönülmesi esastır[cite: 9]. Yanıtın `Location` başlığında (Header) ise yeni oluşturulan kaynağa erişim adresi (`http://localhost:5084/api/Todos/2`) bildirilir[cite: 9, 12].
* **Controller Neden Doğrudan `TodoItem` (Entity) Değil de `TodoResponse` (DTO) Döndürüyor?**
  * Veri katmanına ait iç modelleri (`Entity`) dış dünyaya olduğu gibi açmamak, döngüsel serileştirme sorunlarını önlemek ve istemciye (Client) yalnızca gerekli alanları sunmak için yanıt modelleri (`DTO / Response Model`) kullanılır[cite: 9].

  ---

### 19. Adım: DI Ömürlerini Ölçün (Transient, Scoped, Singleton)

`ITodoRepository` servisinin üç farklı Dependency Injection (DI) yaşam döngüsü ömrü (`Transient`, `Scoped`, `Singleton`) altındaki davranışları, nesne oluşturulma sayıları ve veri tutarlılıkları test edilerek kaydedilmiştir[cite: 9].

#### Ölçüm Tablosu

| Ömür | "Oluşturuldu" Kaç Kez Yazdı (Ölçülen) | GET'te Eklediğiniz Görev Görünüyor mu? (Ölçülen) |
|---|---|---|
| **Transient** | 3 kez | **Hayır** (Her servis çağrısında yeni nesne oluşturuldu) |
| **Scoped** | 3 kez | **Hayır** (Her HTTP isteğinde sıfırdan yeni nesne oluşturuldu) |
| **Singleton** | 1 kez | **Evet** (Tüm isteklerde aynı tek nesne örneği kullanıldı) |

#### Soru & Cevaplar

* **Scoped Kayıtta Eklenen Görev Neden Kayboldu? Gerçek Projelerde (AtisCore / EF Core) Neden Kaybolmaz?**
  * `Scoped` yaşam döngüsünde nesne her HTTP isteği (Request) için sıfırdan bir kez oluşturulur ve istek tamamlandığında bellekten temizlenir[cite: 9]. Veriyi `InMemoryTodoRepository` sınıfı içinde bir C# listesinde (`List<T>`) tuttuğumuz için, nesne silindiğinde listedeki veriler de kaybolmuştur[cite: 9].
  * Gerçek projelerde (AtisCore / EF Core) `DbContext` ve `Dal` sınıfları `Scoped` olarak kaydedilse bile veriler uygulama belleğinde değil, kalıcı **SQL Veritabanında** saklanır[cite: 9]. Nesne her istekte yeniden üretilse de veritabanına bağlanıp veriyi oradan sorguladığı için veriler kaybolmaz[cite: 9].

* **Singleton Bir `List<T>` Yapısını Aynı Anda İki İstek Değiştirmeye Çalışırsa Ne Olur?**
  * Standart C# `List<T>` yapısı iş parçacığı güvenli (thread-safe) değildir[cite: 9]. Aynı anda gelen birden fazla HTTP isteği aynı listeyi değiştirmeye çalıştığında **Race Condition** (Yarış Durumu) oluşur; bu da veri bozulmalarına, indeks kaymalarına veya `ArgumentException` hatalarına yol açar[cite: 9]. Bu nedenle Singleton yapılarda veri tutulacaksa `ConcurrentBag` / `ConcurrentDictionary` gibi thread-safe kolleksiyonlar veya `lock` mekanizmaları kullanılmalıdır[cite: 9].

  ---

### 20. Adım: Doğrulama (Validation)

API'ye gönderilen verilerin biçimsel ve iş kuralı doğrulamaları (Model Validation) test edilmiş, farklı durum kodları ve hata mesajları incelenmiştir.

#### 5 POST İsteğinin İlk Sonuçları

1. **`{ "title": "" }`**: `400 Bad Request` (`Title` alanı boş bırakılamaz kuralı ihlal edildi)[cite: 9].
2. **`{}`**: `400 Bad Request` (`Title` alanı istek gövdesinde bulunamadı)[cite: 9, 25].
3. **150 Karakterlik Başlık**: `400 Bad Request` (`[MaxLength(100)]` kuralı ihlal edildi)[cite: 9, 21].
4. **`{ "title": 123 }`**: `400 Bad Request` (Sayısal değer metin türüne dönüştürülemedi)[cite: 9, 22].
5. **`{ "baslik": "Süt al" }`**: `400 Bad Request` (`title` alanı gelmediği için zorunluluk kuralına takıldı)[cite: 9, 23].

#### `[Required]` Özniteliği Silindiğinde Ne Oldu?

`CreateTodoRequest` tanımındaki `[Required]` özniteliği kaldırılıp 1. ve 2. istekler tekrar atılmıştır[cite: 9]:

* **1. İstek (`{ "title": "" }`):** `400 Bad Request` yanıtı **`500 Internal Server Error`** yanıtına dönüştü[cite: 9, 24]. `[Required]` silindiği için DTO seviyesindeki ön doğrulama geçildi[cite: 9]. Ancak veri domain katmanında `TodoItem` nesnesi oluşturulurken constructor içindeki `string.IsNullOrWhiteSpace` iş kuralına takıldı ve fırlatılan `ArgumentException` sunucu hatasına yol açtı[cite: 8, 24].
* **2. İstek (`{}`):** Yanıt değişmeyerek **`400 Bad Request`** kaldı[cite: 9, 25]. C# `string Title` alanı non-nullable (boş geçilemez) tanımlandığı için ASP.NET Core `[Required]` özniteliği olmasa bile bu alanı örtük olarak zorunlu (`Implicit Required`) kabul etmeye devam etti[cite: 9, 25].

#### Soru & Cevaplar

* **"Başlık boş olamaz" Kuralı Neden İki Yerde De Var? Hangisi Kimin Hatasıdır?**
  * **İstek / DTO Doğrulaması (`CreateTodoRequest` - Validation):** Dış dünyadan (Client) gelen verinin biçimini kontrol eder. Hatalıysa istemciye **`400 Bad Request`** döner (İstemci/Kullanıcı hatası)[cite: 9].
  * **Domain / İş Kuralı Doğrulaması (`TodoItem` Constructor):** Sistemin çekirdek iş kuralıdır. Nesne tutarlılığını korur. Aşılırsa unhandled exception üreterek **`500 Internal Server Error`** oluşturur (Sunucu/Yazılım hatası)[cite: 8, 9, 24].


  ---

### 21. Adım: Log ve Ayarlar (Logging & Configuration)

`TodosController` içerisine `ILogger` enjekte edilerek yapılandırılmış loglama (Structured Logging) mekanizması ve ortam bazlı konfigürasyon (appsettings) hiyerarşisi test edilmiştir.

#### Soru & Cevaplar

* **Log Seviyesi `Warning` Yapıldığında Log Satırı Göründü mü?**
  * **Hayır, görünmedi[cite: 9].** `LogInformation` metodu `Information` seviyesinde log üretir[cite: 9]. Minimum log seviyesi `Warning` olarak ayarlandığında, `Information` seviyesindeki loglar filtrelenerek terminal çıktısına basılmaz[cite: 9].

* **`appsettings.json` ve `appsettings.Development.json` Çelişirse Hangisi Kazanır?**
  * **`appsettings.Development.json` kazanır[cite: 9].** .NET mimarisinde önce genel `appsettings.json` okunur, ardından aktif olan ortama ait dosya (`appsettings.{Environment}.json`) okunarak aynı anahtara sahip ayarların üzerine yazılır (Override)[cite: 9].

* **Uygulama Development Ortamında Çalıştığını Nereden Biliyor?**
  * `Properties/launchSettings.json` dosyası içerisindeki `ASPNETCORE_ENVIRONMENT: "Development"` çevre değişkeninden (Environment Variable) anlar[cite: 9].

* **Log Satırında Neden String Interpolation (`$"Görev eklendi: {item.Id}"`) Yerine Şablon (`"{Id}"`) Kullanıldı?**
  * **Structured Logging (Yapılandırılmış Loglama) için[cite: 9].** String interpolation kullanıldığında metin tek bir düz string olarak birleştirilir. Şablon (`"Görev eklendi: {Id}"`, `item.Id`) kullanıldığında ise loglama sağlayıcıları (Serilog, ElasticSearch, Seq vb.) `Id` alanını ayrıştırılabilir bir veri parametresi olarak saklar. Bu sayede log yönetim panellerinde *"Id'si 5 olan tüm logları getir"* şeklinde filtreleme yapılabilir[cite: 9].


