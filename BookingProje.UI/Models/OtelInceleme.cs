using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingProje.UI.Models
{
    public class OtelInceleme
    {
        public int OtelID { get; set; }
        public string OtelAdi { get; set; }
        public string OtelAciklamasi { get; set; }
        public string OtelIlcesi { get; set; }
        public string OtelSehri { get; set; }
        public decimal NormalOdaFiyati { get; set; }
        public decimal IndirimliOdaFiyati { get; set; }
        public int KisiSayisi { get; set; }
        public string OtelAdresi { get; set; }
        public int OtelYildizi { get; set; }
        public double OtelRating { get; set; }
        public byte[] OdaResmi { get; set; }
        public string OtelTelefonu { get; set; }
        public int OtelinTureGoreOdaSayisi { get; set; }
        public string OdaSinifiOzellikleri { get; set; }
        public List<BookingProje.Data.Tbl_Yorumlar> OtelYorumlari { get; set; }
    }
}