using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StokPanel.Application.Security;
using StokPanel.Domain.Categories;
using StokPanel.Domain.Products;
using StokPanel.Domain.StockMovements;
using StokPanel.Infrastructure.Identity;

namespace StokPanel.Infrastructure.Persistence
{
    // Portföy/demo verisi. Sadece Seed:DemoData=true ve veritabanında hiç ürün yokken çalışır.
    internal static class DemoDataSeeder
    {
        private static readonly string[] OutNotes = ["Satış", "Ofis kullanımı", "Şubeye sevk", "Müşteri siparişi", "Fire"];

        public static async Task SeedAsync(AppDbContext db, UserManager<ApplicationUser> userManager,
            string adminId, string personelEmail, string password)
        {
            if (await db.Products.AnyAsync())
                return;

            // Önce kullanıcı: UserManager kendi SaveChanges'ini çağırır, o anda bekleyen başka kayıt olmasın
            var personel = await EnsurePersonelAsync(userManager, personelEmail, password);

            var categories = new Dictionary<string, Category>
            {
                ["Kırtasiye"] = Category.Create("Kırtasiye", "Kalem, kağıt ve ofis sarf malzemeleri"),
                ["Temizlik"] = Category.Create("Temizlik", "Temizlik ve hijyen ürünleri"),
                ["Mutfak"] = Category.Create("Mutfak", "Çay, kahve ve ikram malzemeleri"),
                ["Elektronik"] = Category.Create("Elektronik", "Kablo, bellek ve küçük ekipmanlar"),
            };
            db.Categories.AddRange(categories.Values);

            (string Name, string Sku, string Unit, string Category, decimal Critical, decimal Target)[] items =
            [
                ("Kurşun Kalem HB", "KRT-001", "adet", "Kırtasiye", 50, 240),
                ("A4 Fotokopi Kağıdı", "KRT-002", "paket", "Kırtasiye", 20, 36),
                ("Spiralli Defter", "KRT-003", "adet", "Kırtasiye", 30, 18),        // kritik
                ("Zımba Teli No:10", "KRT-004", "kutu", "Kırtasiye", 10, 42),
                ("Sıvı Sabun", "TMZ-001", "lt", "Temizlik", 15, 48.5m),
                ("Kağıt Havlu", "TMZ-002", "koli", "Temizlik", 5, 3),               // kritik
                ("Yüzey Temizleyici", "TMZ-003", "lt", "Temizlik", 10, 22),
                ("Türk Kahvesi", "MTF-001", "kg", "Mutfak", 2, 6.5m),
                ("Siyah Çay", "MTF-002", "kg", "Mutfak", 5, 4.5m),                 // kritik
                ("Küp Şeker", "MTF-003", "kg", "Mutfak", 3, 12),
                ("USB Bellek 32 GB", "ELK-001", "adet", "Elektronik", 5, 14),
                ("HDMI Kablo 2 m", "ELK-002", "adet", "Elektronik", 3, 2),         // kritik
            ];

            var random = new Random(42);   // Sabit tohum: her kurulumda aynı demo verisi
            var nowUtc = DateTime.UtcNow;

            foreach (var item in items)
            {
                var product = Product.Create(item.Name, item.Sku, item.Unit, categories[item.Category].Id, item.Critical);
                db.Products.Add(product);

                var decimals = item.Unit is "kg" or "lt" ? 1 : 0;
                var outs = Enumerable.Range(0, random.Next(3, 7))
                    .Select(_ => Math.Max(1, Math.Round(item.Target * (decimal)(0.08 + random.NextDouble() * 0.2), decimals)))
                    .ToList();

                // Açılış girişi = hedef stok + çıkışların toplamı → çıkışlardan sonra stok hedefe iner
                var opening = StockMovement.CreateIn(product, item.Target + outs.Sum(), adminId, "Açılış sayımı");
                AddWithDate(db, opening, nowUtc.AddDays(-30).AddHours(-2));

                // Çıkışlar son 4 haftaya dağılsın, sonuncusu bugün (göstergede "bugün" dolu görünsün)
                var daysAgo = outs.Select((_, i) => i == outs.Count - 1 ? 0 : random.Next(1, 29))
                    .OrderDescending()
                    .ToList();

                for (var i = 0; i < outs.Count; i++)
                {
                    var movement = StockMovement.CreateOut(product, outs[i], personel.Id, OutNotes[random.Next(OutNotes.Length)]);
                    AddWithDate(db, movement, nowUtc.AddDays(-daysAgo[i]).AddMinutes(-random.Next(5, 240)));
                }
            }

            await db.SaveChangesAsync();
        }

        // Domain tarihi "şimdi" olarak verir; demo için geçmişe çekiyoruz (sadece seed'de)
        private static void AddWithDate(AppDbContext db, StockMovement movement, DateTime createdAtUtc)
        {
            db.StockMovements.Add(movement);
            db.Entry(movement).Property(m => m.CreatedAtUtc).CurrentValue = createdAtUtc;
        }

        private static async Task<ApplicationUser> EnsurePersonelAsync(UserManager<ApplicationUser> userManager, string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is not null)
                return user;

            user = new ApplicationUser { UserName = email, Email = email, FullName = "Ayşe Yılmaz", EmailConfirmed = true };
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new InvalidOperationException("Demo personel oluşturulamadı: " + string.Join(" ", result.Errors.Select(e => e.Description)));

            await userManager.AddToRoleAsync(user, Roles.Personel);
            return user;
        }
    }
}