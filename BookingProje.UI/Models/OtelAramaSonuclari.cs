using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingProje.UI.Models
{
    public class OtelAramaSonuclari
    {
        public int ilceID { get; set; }
        public int otelYildizi { get; set; }
        public DateTime girisTarihi { get; set; }
        public DateTime cikisTarihi { get; set; }
        public int kisiSayisi { get; set; }
        public int OdaSinifiID { get; set; }
    }
}