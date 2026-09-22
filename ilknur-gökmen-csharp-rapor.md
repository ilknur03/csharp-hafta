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