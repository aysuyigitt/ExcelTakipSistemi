using ExcelSatısVerileri.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExcelSatısVerileri.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SatislarController : ControllerBase
    {
        private readonly SatislarService _satislarService;

        public SatislarController(SatislarService satislarService)
        {
            _satislarService = satislarService;
        }
        //GET endpoint ile satış verilerini API üzerinden dış dünyaya açtım.
        [HttpGet]
        public IActionResult GetSatislar()
        {
            var value = _satislarService.GetAllSatislar();
            return Ok(value);
        }

        [HttpGet("getFilteredSatislar")]
        public IActionResult GetFilteredSatislar(string? search, int page= 1, int pageSize= 30)
        {
            var value = _satislarService.GetFilteredSatislar(search, page, pageSize);
            return Ok(value);
        }

        //Kullanıcı Excel dosyasını gönderir, API bu dosyayı alır, Service’e gönderir
        [HttpPost]
        public IActionResult UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Dosya yüklenemedi");

            var filePath = Path.Combine(Path.GetTempPath(), file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            _satislarService.ImportExcel(filePath); //Controller sadece isteği alıp Service’e yönlendirir.
            return Ok("Excel başarıyla yüklendi ve veriler kaydedildi.");
        }
    }
}
