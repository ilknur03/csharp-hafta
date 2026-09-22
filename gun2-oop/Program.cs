ITodoRepository repo = new JsonFileTodoRepository("todos.json");

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
            try
            {
                repo.Add(baslik ?? "");
                Console.WriteLine("Görev eklendi.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
            }
            break;

        case "2":
            var tumu = repo.GetAll();
            if (tumu.Count == 0)
            {
                Console.WriteLine("Listenizde henüz görev yok.");
            }
            else
            {
                foreach (var item in tumu)
                {
                    var isaret = item.IsDone ? "[x]" : "[ ]";
                    Console.WriteLine($"ID: {item.Id} | {isaret} {item.Title} (Oluşturulma: {item.CreatedAt:HH:mm:ss})");
                }
            }
            break;

        case "3":
            Console.Write("Tamamlanacak Görev ID: ");
            if (int.TryParse(Console.ReadLine(), out int tamamlaId) && repo.Complete(tamamlaId))
            {
                Console.WriteLine($"{tamamlaId} ID'li görev tamamlandı.");
            }
            else
            {
                Console.WriteLine("Görev bulunamadı veya geçersiz ID!");
            }
            break;

        case "4":
            Console.Write("Silinecek Görev ID: ");
            if (int.TryParse(Console.ReadLine(), out int silId) && repo.Remove(silId))
            {
                Console.WriteLine($"{silId} ID'li görev silindi.");
            }
            else
            {
                Console.WriteLine("Görev bulunamadı veya geçersiz ID!");
            }
            break;

        case "0":
            Console.WriteLine("Çıkılıyor...");
            return;

        default:
            Console.WriteLine("Geçersiz seçim!");
            break;
    }
}
