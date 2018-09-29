using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingProje.UI.Models
{
    public class RezervasyonBilgileri
    {
        public string OtelAdi { get; set; }
        public string OtelAdresi { get; set; }
        public string OtelTelefonu { get; set; }
        public int KisiSayisi { get; set; }
        public byte[] OtelResmi { get; set; }
        public decimal Tutar { get; set; }
        public int OdaNumarasi { get; set; }
        public DateTime GirisTarihi { get; set; }
        public DateTime CikisTarihi { get; set; }


    }
}