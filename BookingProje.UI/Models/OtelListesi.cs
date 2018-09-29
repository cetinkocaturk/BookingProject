using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingProje.UI.Models
{
    public class OtelListesi
    {
        public int OtelID { get; set; }
        public byte[] OtelResmi { get; set; }
        public int OtelYildizi { get; set; }
        public string OtelAdi { get; set; }
        public string OtelAciklamasi { get; set; }
        public string OtelIlcesi { get; set; }
        public string OtelSehri { get; set; }
        public decimal OtelOdaFiyatiIndirim { get; set; }
        public decimal OtelOdaFiyatiNormal { get; set; }
        public bool OtelWifi { get; set; }
    }
}