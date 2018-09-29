using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingProje.UI.Models
{
    public class OtelIstatistik
    {
        public string GorevliAdi { get; set; }
        public string GorevliSoyadi { get; set; }
        public int MusteriSayisi { get; set; }
        public int BosOdaSayisi { get; set; }
        public int DoluOdaSayisi { get; set; }
        public int ToplamOdaSayisi { get; set; }
    }
}