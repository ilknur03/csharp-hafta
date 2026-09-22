var gorevler = new List<string>();
var tamamlananlar = new HashSet<int>();

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1) Ekle  2) Listele  3) Tamamla  4) Sil  0) Çıkış");
    Console.Write("Seçim: ");
    var secim = Console.ReadLine();

    switch (secim)
    {
        case "1":
            Console.Write("Başlık: ");
            var baslik = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(baslik))
            {
                gorevler.Add(baslik);
                Console.WriteLine("Görev eklendi.");
            }
            else
            {
                Console.WriteLine("Boş görev başlığı eklenemez!");
            }
            break;

        case "2":
            if (gorevler.Count == 0)
            {
                Console.WriteLine("Listenizde henüz görev yok.");
            }
            else
            {
                for (int i = 0; i < gorevler.Count; i++)
                {
                    var isaret = tamamlananlar.Contains(i) ? "[x]" : "[ ]";
                    Console.WriteLine($"{i}. {isaret} {gorevler[i]}");
                }
            }
            break;

        case "3":
            Console.Write("Tamamlanacak görev numarası: ");
            var tamamlaGirdi = Console.ReadLine();

            // Kullanıcı harf girerse veya listede olmayan bir numara girerse program çökmez
            if (int.TryParse(tamamlaGirdi, out int tamamlaIndex) && tamamlaIndex >= 0 && tamamlaIndex < gorevler.Count)
            {
                tamamlananlar.Add(tamamlaIndex);
                Console.WriteLine($"{tamamlaIndex}. numaralı görev tamamlandı olarak işaretlendi.");
            }
            else
            {
                Console.WriteLine("Geçersiz veya bulunamayan bir görev numarası girdiniz!");
            }
            break;

        case "4":
            Console.Write("Silinecek görev numarası: ");
            var silGirdi = Console.ReadLine();

            if (int.TryParse(silGirdi, out int silIndex) && silIndex >= 0 && silIndex < gorevler.Count)
            {
                gorevler.RemoveAt(silIndex);

                // HashSet içindeki indeksleri güncellemek/temizlemek için sıfırlıyoruz
                tamamlananlar.Remove(silIndex);

                Console.WriteLine($"{silIndex}. numaralı görev silindi.");
            }
            else
            {
                Console.WriteLine("Geçersiz veya bulunamayan bir görev numarası girdiniz!");
            }
            break;

        case "0":
            Console.WriteLine("Programdan çıkılıyor...");
            return;

        default:
            Console.WriteLine("Lütfen geçerli bir menü seçeneği (0-4) giriniz.");
            break;
    }
}