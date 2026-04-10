using ExcelSatısVerileri.Service;
using ExcelSatısVerileriUI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace ExcelSatısVerileriUI.Controllers
{
    public class SatisController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SatisController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Listeleme
         public async Task<IActionResult> SatisList(string search, int pageNumber = 1, int pageSize = 30)
        { 
            var client = _httpClientFactory.CreateClient(); // API endpoint (filtreli ve sayfalı)
            var url = $"http://localhost:36347/api/Satislar/getFilteredSatislar" + $"?search={search}&page={pageNumber}&pageSize={pageSize}";
            var response = await client.GetAsync(url); 
            List<ResultSatisDto> satislar = new(); 

            if (response.IsSuccessStatusCode) 
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                satislar = JsonConvert.DeserializeObject<List<ResultSatisDto>>(jsonData); 
            } 

            ViewBag.CurrentPage = pageNumber; 
            ViewBag.PageSize = pageSize;
            ViewBag.Search = search;
            return View(satislar); 
        }

        // Excel yükleme
        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Dosya yüklenemedi");

            var client = _httpClientFactory.CreateClient();
            var formData = new MultipartFormDataContent();
            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            formData.Add(streamContent, "file", file.FileName);

            await client.PostAsync("http://localhost:36347/api/Satislar", formData);

            var response = await client.GetAsync("http://localhost:36347/api/Satislar");
            List<ResultSatisDto> satislar = new List<ResultSatisDto>();
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                satislar = JsonConvert.DeserializeObject<List<ResultSatisDto>>(jsonData);
            }

            ViewBag.Message = "Excel başarıyla yüklendi ve veriler güncellendi.";
            return View("SatisList", satislar);
        }

    }
}

/*Kullanıcı arayüzden Excel dosyasını seçtikten sonra “Excel Yükle” butonuna tıklayarak formu gönderir. Bu işlemle birlikte dosya,
 * MVC Controller’daki UploadExcel metoduna POST edilir. Controller, dosyayı alır ve HttpClient aracılığıyla API’ye gönderir. API tarafında, dosya açılır, satır satır kontrol edilir ve validasyonlardan
 * geçirilir; ardından her bir satır Satis entity’sine dönüştürülerek Entity Framework Core ile veritabanına kaydedilir. Excel yükleme işlemi tamamlandıktan sonra MVC Controller, API’den güncellenmiş tüm 
 * satış verilerini alır ve View’a gönderir. Böylece kullanıcı hem “Excel başarıyla yüklendi” şeklinde bir geri bildirim mesajını görür hem de veritabanına eklenen yeni verilerin tablo hâlinde güncel 
 * listesini görüntüleyebilir.*/