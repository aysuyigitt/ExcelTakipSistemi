using ExcelSatısVerileri.Context;
using ExcelSatısVerileri.Dtos;
using ExcelSatısVerileri.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Packaging.Ionic.Zlib;
using System.Drawing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ExcelSatısVerileri.Service
{
    public class SatislarService
    {
        private readonly ApiContext _apiContext;

        public SatislarService(ApiContext apiContext)
        {
            _apiContext = apiContext;
        }

        public void ImportExcel(string filePath)
        {
            var allOldSatis = _apiContext.Satislar.ToList();
            if (allOldSatis.Any()) 
            {
                _apiContext.Satislar.RemoveRange(allOldSatis);
                _apiContext.SaveChanges(); 
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                if (!DateTime.TryParse(worksheet.Cells[row, 1].Text, out var tarih)) continue;
                var urunAdi = worksheet.Cells[row, 2].Text;
                var kategori = worksheet.Cells[row, 3].Text;
                if (!int.TryParse(worksheet.Cells[row, 4].Text, out var adet)) continue;
                if (!decimal.TryParse(worksheet.Cells[row, 5].Text, out var birimFiyat)) continue;

                var satis = new Satis
                {
                    Tarih = tarih,
                    UrunAdi = urunAdi,
                    Kategori = kategori,
                    Adet = adet,
                    BirimFiyat = birimFiyat,
                    ToplamTutar = adet * birimFiyat
                };
                _apiContext.Satislar.Add(satis);
            }
            _apiContext.SaveChanges();
        }

        public List<Satis> GetAllSatislar()
        {
            return _apiContext.Satislar.ToList();
        }


        public List<Satis> GetFilteredSatislar(string? search, int page, int pageSize)
        {
            var query = _apiContext.Satislar.AsQueryable();


            if (!string.IsNullOrWhiteSpace(search)) //Kullanıcı gerçekten bir şey yazmış mı
            {
                query = query.Where(x => //Sadece sorguya bir koşul ekler SQL WHERE hazırlığı yapar
                    x.UrunAdi.Contains(search) || //Ürün adında aranan kelime geçiyor mu diye bakar
                    x.Kategori.Contains(search)); //Kategori alanında aranan kelime geçiyor mu diye baka
            }

            return query
                .OrderByDescending(x => x.Tarih)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }
}



/*🔁 Excel → Database Veri Akışı Açıklaması

Bu projede Excel’den gelen verileri doğrudan veritabanına yazmak yerine, kontrollü ve katmanlı bir veri akışı oluşturdum.

1️⃣ Excel (string)

Excel dosyasındaki tüm hücreler uygulamaya string olarak gelir. Bu veriler kullanıcı tarafından düzenlenebildiği için
format hataları, boş değerler veya yanlış tipler içerebilir. Bu nedenle Excel, güvenilir olmayan bir dış veri 
kaynağıdır ve doğrudan veritabanına yazılması doğru değildir.

2️⃣ Validation (TryParse)

Excel’den gelen string verileri TryParse yöntemleriyle doğruladım. Tarih, sayı ve para gibi alanların doğru formatta 
olup olmadığını kontrol ettim. Hatalı olan satırları exception fırlatmadan atlayarak sistemin çalışmaya devam etmesini 
sağladım. Bu adım uygulamayı hatalı verilerden koruyan bir güvenlik katmanı görevi görür.

3️⃣ Entity (Satis)

Doğrulanan ve temizlenen verileri, veritabanındaki satış tablosunu temsil eden Satis entity modeline dönüştürdüm. 
Entity, hem veri tiplerini hem de iş kurallarını temsil ettiği için yalnızca geçerli verilerle oluşturuldu. 
Böylece veritabanına gidecek verinin anlamlı ve tutarlı olmasını sağladım.

4️⃣ DbContext (EF Core)

Oluşturulan entity nesnelerini DbContext aracılığıyla Entity Framework Core’a ekledim. DbContext, 
entity’leri takip ederek hangi tabloya ve hangi kolonlara yazılacağını belirler, transaction yönetimini sağlar ve 
veritabanı ile uygulama arasındaki köprü görevini üstlenir.

5️⃣ Database

Son aşamada SaveChanges() ile tüm entity’ler tek bir transaction içerisinde veritabanına kaydedilir. 
Bu sayede veri bütünlüğü korunur ve Excel’den gelen doğru veriler kalıcı hale getirilir.

Get()
Bu metot Entity Framework Core üzerinden satış tablosundaki tüm kayıtları çekip liste olarak döndürür.

GetFilteredSatislar

AsQueryable, sorguyu hemen çalıştırmadan, veritabanına gönderilecek sorguyu adım adım oluşturmamıza izin veren bir yapıdı.
“Bu bir sorgu taslağıdır, henüz çalıştırma.
Üzerine filtre, sıralama, sayfalama eklenebilir.”
ToList BAŞTA OLURSA NE OLUR?
var list = _apiContext.Satislar.ToList();

Bu şu demek:

“Ne varsa getir, sonra ben bakarım.”

🚨 Büyük veri = RAM problemi

Eğer AsQueryable kullanmayıp en başta ToList ile tüm veriyi çekseydim, filtreleme ve sayfalama işlemleri bellek üzerinde gerçekleşirdi.
Bu da büyük veri setlerinde ciddi performans problemlerine, gereksiz bellek ve network kullanımına yol açardı. 
Bu nedenle sorguyu AsQueryable ile oluşturarak filtreleme, sıralama ve sayfalama işlemlerini veritabanı seviyesinde yaptım ve ToList çağrısını en sona bıraktım.”
❌ ESKİ YAPIDA NE OLUYORDU?
allSatislar.Where(...).ToList();

Önce her şey geliyordu

Sonra C# belleğinde filtreleniyordu

Büyük veri = yavaşlık

 
 */