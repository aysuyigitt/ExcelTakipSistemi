using System.ComponentModel.DataAnnotations;

namespace ExcelSatısVerileri.Entities
{
    public class Satis
    {
        public int SatisID {  get; set; }

        public DateTime Tarih { get; set; }

        [Required]
        [MaxLength(150)]
        public string UrunAdi { get; set; }

        [Required]
        [MaxLength(100)]
        public string Kategori { get; set; }

        public int Adet { get; set; }

        public decimal BirimFiyat { get; set; }

        public decimal ToplamTutar { get; set; }
    }
}
