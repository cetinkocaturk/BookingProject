using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingProje.Data;

namespace BookingProje.Business
{
    public class HotelOperation
    {
        BookingProjectEntities db;
        public HotelOperation()
        {
            db = new BookingProjectEntities();
        }
        public Tbl_Oteller OtelGetir(int gorevliID, string otelKodu)
        {
            return (from i in db.Tbl_Oteller where i.GorevliID == gorevliID && i.OtelKodu == otelKodu && i.IsActive.Value select i).Single();
        }
        public int? MusteriSayisi(int otelID)
        {
            int? musteriSayisi = (from i in db.Tbl_Rezervasyonlar where i.Tbl_Odalar.OtelID == otelID && i.CikisTarihi >= DateTime.Now && i.Tbl_Odalar.IsActive.Value == false select i).Sum(s => s.KisiSayisi);
            if (musteriSayisi == null)
                return 0;
            else
                return musteriSayisi;
        }
        public int BosOdaSayisi(int otelID)
        {
            int bosOdaSayisi = (from i in db.Tbl_Odalar where i.OtelID == otelID && i.IsActive.Value select i).Count();
            if (bosOdaSayisi.Equals(null))
                return 0;
            else
                return bosOdaSayisi;
        }
        public int DoluOdaSayisi(int otelID)
        {
            int doluOdaSayisi = (from i in db.Tbl_Odalar where i.OtelID == otelID && i.IsActive.Value == false select i).Count();
            if (doluOdaSayisi.Equals(null))
                return 0;
            else
                return doluOdaSayisi;
        }
        public List<Tbl_Odalar> OdalariGetir(int otelid, string otelkodu)
        {
            return (from i in db.Tbl_Odalar where i.OtelID == otelid && i.Tbl_Oteller.OtelKodu == otelkodu select i).ToList();
        }
        public Tbl_Odalar TekliOdaGetir(int odaid)
        {
            return (from i in db.Tbl_Odalar where i.OdaID == odaid select i).First();
        }
        public List<Tbl_OdaTurleri> OdaTuruListesi()
        {
            return (from i in db.Tbl_OdaTurleri select i).ToList();
        }
        public int OdaEkle(int otelid, int odaturuid, int odanumarasi, decimal normalodafiyati, decimal indirimliodafiyati, byte[] odaresmi)
        {
            Tbl_Odalar odaEkle = new Tbl_Odalar
            {
                OtelID = otelid,
                OdaTuruID = odaturuid,
                OdaNumarasi = odanumarasi,
                NormalOdaFiyati = normalodafiyati,
                IndirimliOdaFiyati = indirimliodafiyati,
                OdaResmi = odaresmi,
                IsActive = true,
                CreatedDate = DateTime.Now
            };
            db.Tbl_Odalar.Add(odaEkle);
            db.SaveChanges();
            return odaEkle.OdaID;
        }
        public string OdaSil(int odaID)
        {
            bool rezVarMi = (from i in db.Tbl_Rezervasyonlar where i.OdaID == odaID select i).Any();
            if (rezVarMi)
            {
                return "Oda rezerv olduğu için silinemez.";
            }
            else
            {
                var sil = (from i in db.Tbl_Odalar where i.OdaID == odaID select i).SingleOrDefault();
                db.Tbl_Odalar.Remove(sil);
                db.SaveChanges();
                return "Oda silindi.";
            }
        }
    }
}
