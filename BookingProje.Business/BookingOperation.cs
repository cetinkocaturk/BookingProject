using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingProje.Data;

namespace BookingProje.Business
{
    public class BookingOperation
    {
        BookingProjectEntities db;
        public BookingOperation()
        {
            db = new BookingProjectEntities();
        }
        public List<Tbl_Sehirler> SehirListesi()
        {
            return (from i in db.Tbl_Sehirler select i).ToList();
        }
        public List<Tbl_Ilceler> IlceListesi(int ID)
        {
            return (from i in db.Tbl_Ilceler where i.SehirID == ID select i).ToList();
        }
        public Tbl_Kullanicilar KullaniciGetir(string email, string sifre)
        {
            return (from i in db.Tbl_Kullanicilar where i.KullaniciEmail.Equals(email) && i.KullaniciSifre.Equals(sifre) select i).Single();
        }
        public bool KullaniciVarMi(string email, string sifre)
        {
            return (from i in db.Tbl_Kullanicilar where i.KullaniciEmail.Equals(email) && i.KullaniciSifre.Equals(sifre) select i).Any();
        }
        public bool KullaniciKaydiVarMi(string email, string telefon)
        {
            var varMi = (from i in db.Tbl_Kullanicilar where i.KullaniciEmail.Equals(email) || i.KullaniciTelefon.Equals(telefon) select i).Any();
            if (varMi)
                return true;
            else
                return false;
        }
        public bool KullaniciKaydi(string ad, string soyad, string email, string sifre, string telefon)
        {
            if (string.IsNullOrEmpty(ad) || string.IsNullOrEmpty(soyad) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(sifre) || string.IsNullOrEmpty(telefon))
            {
                return false;
            }
            else
            {
                Tbl_Kullanicilar kullaniciEkle = new Tbl_Kullanicilar
                {
                    KullaniciAdi = ad,
                    KullaniciSoyadi = soyad,
                    KullaniciEmail = email,
                    KullaniciSifre = sifre,
                    KullaniciTelefon = telefon,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                };
                db.Tbl_Kullanicilar.Add(kullaniciEkle);
                db.SaveChanges();
                return true;
            }
        }
        public Tbl_Gorevliler GorevliGetir(string email, string sifre, string otelkodu)
        {
            return (from i in db.Tbl_Gorevliler join x in db.Tbl_Oteller on i.GorevliID equals x.GorevliID where i.GorevliEmail.Equals(email) && i.GorevliSifre.Equals(sifre) && x.OtelKodu.Equals(otelkodu) && i.IsActive.Value && x.IsActive.Value select i).Single();
        }
        public bool GorevliVarMi(string email, string sifre)
        {
            var varMi = (from i in db.Tbl_Gorevliler where i.GorevliEmail.Equals(email) && i.GorevliSifre.Equals(sifre) select i).Any();
            if (varMi)
                return true;
            else
                return false;
        }
        public bool OtelBasvurusu(string otelAdi, string adres, int ilceid, int otelYildizi, int odaSayisi, string gorevliAdi, string gorevliSoyadi, string email, string sifre, string telefon)
        {
            if (string.IsNullOrEmpty(otelAdi) || string.IsNullOrEmpty(adres) || ilceid.Equals(null) || otelYildizi.Equals(null) || odaSayisi.Equals(null) || string.IsNullOrEmpty(gorevliAdi) || string.IsNullOrEmpty(gorevliSoyadi) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(sifre) || string.IsNullOrEmpty(telefon))
                return false;
            else
            {
                Tbl_OtelBasvuru otelBasvuru = new Tbl_OtelBasvuru
                {
                    OtelAdi = otelAdi,
                    Adres = adres,
                    IlceID = ilceid,
                    OtelYildizi = otelYildizi,
                    OdaSayisi = odaSayisi,
                    GorevliAdi = gorevliAdi,
                    GorevliSoyadi = gorevliSoyadi,
                    GorevliEmail = email,
                    GorevliSifre = sifre,
                    GorevliTelefon = telefon,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };
                db.Tbl_OtelBasvuru.Add(otelBasvuru);
                db.SaveChanges();
                return true;
            }
        }
        public List<Tbl_Oteller> OtelListesi(int ilceID)
        {
            return (from i in db.Tbl_Oteller where i.İlceID == ilceID && i.IsActive.Value select i).ToList();
        }
        public Tbl_Oteller OtelGetir(int id)
        {
            return (from i in db.Tbl_Oteller where i.OtelID == id && i.IsActive.Value select i).SingleOrDefault();
        }
        public List<Tbl_Odalar> OdaTuruListesi(int otelid)
        {
            return (from i in db.Tbl_Odalar where i.OtelID == otelid && i.IsActive.Value select i).ToList();
            //return (from i in db.Tbl_Odalar where i.OtelID == otelid && i.IsActive.Value group i.Tbl_OdaTurleri by i.Tbl_OdaTurleri.OdaTuru into g select new { OdaTuru = g.Key, OdaTuruID = g.OrderBy(v => v.OdaTuruID).FirstOrDefault() }).ToList();
        }
        public List<Tbl_Yorumlar> OtelYorumListesi(int otelid)
        {
            return (from i in db.Tbl_Yorumlar where i.OtelID == otelid && i.IsActive.Value select i).ToList();
        }
        public double OtelRatingOrtalama(int otelid)
        {
            double? ortalama = (from i in db.Tbl_Puanlamalar where i.OtelID == otelid && i.IsActive.Value select i).Average(s => s.Puan);
            if (ortalama == null)
                return 0;
            else
                return (double)ortalama;
        }
        public List<Tbl_Odalar> OdaNumaraList(int otelID, int odaTuruID)
        {
            return (from i in db.Tbl_Odalar where i.OtelID == otelID && i.OdaTuruID == odaTuruID && i.IsActive.Value select i).ToList();
        }
        public decimal? OtelinEnUcuzOdaFiyati(int otelid)
        {
            return (from i in db.Tbl_Odalar where i.OtelID == otelid select i.IndirimliOdaFiyati).Min();
        }
        public decimal? OtelinEnPahaliOdaFiyati(int otelid)
        {
            return (from i in db.Tbl_Odalar where i.OtelID == otelid select i.NormalOdaFiyati).Max();
        }
        public int OtelinOdaSinifininOdaSayisi(int otelid, int odaturuid)
        {
            int odaSayisi = (from i in db.Tbl_Odalar where i.OtelID == otelid && i.OdaTuruID == odaturuid && i.IsActive.Value select i).Count();
            if (odaSayisi.Equals(null))
                return 0;
            else
                return odaSayisi;
        }
        public string OdaSinifiOzellikleri(int odaturuid)
        {
            return (from i in db.Tbl_OdaTurleri where i.OdaTuruID == odaturuid select i.OdaAciklamasi).Single();
        }
        public int RezervasyonYap(int kullaniciid, int odaid, int kisisayisi, decimal odenentutar, DateTime giristarihi, DateTime cikistarihi)
        {
            Tbl_Rezervasyonlar rezerveOda = new Tbl_Rezervasyonlar
            {
                KullaniciID = kullaniciid,
                OdaID = odaid,
                KisiSayisi = kisisayisi,
                OdenenTutar = odenentutar,
                GirisTarihi = giristarihi,
                CikisTarihi = cikistarihi,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            db.Tbl_Rezervasyonlar.Add(rezerveOda);
            db.SaveChanges();
            return rezerveOda.RezerveOdaID;
        }
        public void OteleYorumYap(int otelid, int kullaniciid, string yorum)
        {
            Tbl_Yorumlar kullaniciYorum = new Tbl_Yorumlar
            {
                KullaniciID = kullaniciid,
                OtelID = otelid,
                Yorum = yorum,
                IsActive = false,
                CreatedDate = DateTime.Now
            };
            db.Tbl_Yorumlar.Add(kullaniciYorum);
            db.SaveChanges();
        }
        public void OtelePuanVer(int otelid, int kullaniciid, int puan)
        {
            Tbl_Puanlamalar kullaniciPuan = new Tbl_Puanlamalar
            {
                KullaniciID = kullaniciid,
                OtelID = otelid,
                Puan = puan,
                IsActive = true,
                CreatedDate = DateTime.Now
            };
            db.Tbl_Puanlamalar.Add(kullaniciPuan);
            db.SaveChanges();
        }
        public string OtelAdi(int otelid)
        {
            return (from i in db.Tbl_Oteller where i.OtelID == otelid select i.OtelAdi).Single();
        }
        public string OtelAdresi(int otelid)
        {
            return (from i in db.Tbl_Oteller where i.OtelID == otelid select i.AcikAdres).Single();
        }
    }
}
