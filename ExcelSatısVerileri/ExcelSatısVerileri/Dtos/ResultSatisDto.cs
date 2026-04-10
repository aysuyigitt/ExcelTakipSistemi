namespace ExcelSatısVerileri.Dtos
{
    public class ResultSatisDto
    {
        public int SatisID { get; set; }
        public DateTime Tarih { get; set; }
        public string UrunAdi { get; set; }
        public string Kategori { get; set; }
        public int Adet { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal ToplamTutar { get; set; }
    }
}
