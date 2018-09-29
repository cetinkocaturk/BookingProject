using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingProje.Business;
using BookingProje.Data;
using System.Net.Mail;
using System.Text;
using System.Net;

namespace BookingProje.UI.Controllers
{
    public class HomeController : Controller
    {
        BookingOperation op;
        BookingProjectEntities db;
        HotelOperation ho;
        public HomeController()
        {
            op = new BookingOperation();
            db = new BookingProjectEntities();
            ho = new HotelOperation();
        }
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult SelectOtel()
        {
            SelectList odaTuruListe = new SelectList(ho.OdaTuruListesi(), "OdaTuruID", "OdaTuru");
            ViewBag.OdaTurleri = odaTuruListe;
            SelectList sehirler = new SelectList(op.SehirListesi(), "SehirID", "SehirAdi");
            ViewBag.Sehirler = sehirler;
            return PartialView();
        }
        public JsonResult IlceGetir(int ID)
        {
            SelectList ilceler = new SelectList(op.IlceListesi(ID), "IlceID", "IlceAdi");
            return Json(ilceler, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult OtelSonuclari(FormCollection frm)
        {
            List<Models.OtelAramaSonuclari> model = new List<Models.OtelAramaSonuclari>();
            model.Add(new Models.OtelAramaSonuclari()
            {
                ilceID = int.Parse(frm["ddlIlceler"]),
                otelYildizi = int.Parse(frm["ddlYildizlar"]),
                kisiSayisi = int.Parse(frm["numbKisiSayisi"]),
                cikisTarihi = DateTime.Parse(frm["txtCikisTarihi"]),
                girisTarihi = DateTime.Parse(frm["txtGirisTarihi"]),
                OdaSinifiID = int.Parse(frm["ddlOdaTurleri"])
            });
            string yildizSayisi = frm["ddlYildizlar"].ToString();
            Session["HotelResult"] = model;
            int ilceID = int.Parse(frm["ddlIlceler"]);
            int odaSinifiID = int.Parse(frm["ddlOdaTurleri"]);

            var otelList = (from i in db.Tbl_Oteller
                            where i.İlceID == ilceID && i.IsActive.Value && i.OtelYildizi == yildizSayisi
                            select new
                            {
                                otelID = i.OtelID,
                                otelResmi = i.OtelResmi,
                                otelYildizi = i.OtelYildizi,
                                otelAdi = i.OtelAdi,
                                otelAciklamasi = i.OtelAciklamasi,
                                otelIlcesi = i.Tbl_Ilceler.IlceAdi,
                                otelSehri = i.Tbl_Ilceler.Tbl_Sehirler.SehirAdi,
                                otelOdaFiyatiMin = i.Tbl_Odalar.Where(s => s.OdaTuruID == odaSinifiID).Min(s => s.IndirimliOdaFiyati),
                                otelOdaFiyatiMax = i.Tbl_Odalar.Where(s => s.OdaTuruID == odaSinifiID).Max(s => s.NormalOdaFiyati),
                                otelWifi = i.UcretsizWifi
                            }).ToList();

            List<Models.OtelListesi> oteller = new List<Models.OtelListesi>();
            foreach (var otel in otelList)
            {
                oteller.Add(new Models.OtelListesi()
                {
                    OtelID = otel.otelID,
                    OtelAciklamasi = otel.otelAciklamasi,
                    OtelAdi = otel.otelAdi,
                    OtelIlcesi = otel.otelIlcesi,
                    OtelSehri = otel.otelSehri,
                    OtelResmi = otel.otelResmi,
                    OtelYildizi = int.Parse(otel.otelYildizi),
                    OtelOdaFiyatiIndirim = otel.otelOdaFiyatiMin.Equals(null) ? 0 : (decimal)otel.otelOdaFiyatiMin,
                    OtelOdaFiyatiNormal = otel.otelOdaFiyatiMax.Equals(null) ? 0 : (decimal)otel.otelOdaFiyatiMax,
                    OtelWifi = (bool)otel.otelWifi
                });
            }
            return View(oteller);
        }
        public ActionResult OtelSonuclari()
        {
            return View();
        }
        public ActionResult OteliIncele(int otelid)
        {
            Session["HotelID"] = otelid;
            int odaSinifiID = ((List<Models.OtelAramaSonuclari>)Session["HotelResult"]).First().OdaSinifiID;
            int otelinOdaSinifiOdaSayisi = op.OtelinOdaSinifininOdaSayisi(otelid, odaSinifiID);
            int kisiSayisi = ((List<Models.OtelAramaSonuclari>)Session["HotelResult"]).First().kisiSayisi;
            string odaSinifiOzellikleri = op.OdaSinifiOzellikleri(odaSinifiID);
            double otelRatingOrtalama = op.OtelRatingOrtalama(otelid);
            var otelDetay = (from i in db.Tbl_Oteller
                             where i.OtelID == otelid && i.IsActive.Value
                             select new
                             {
                                 otelID = i.OtelID,
                                 otelAdi = i.OtelAdi,
                                 otelAciklamasi = i.OtelAciklamasi,
                                 otelIlcesi = i.Tbl_Ilceler.IlceAdi,
                                 otelSehri = i.Tbl_Ilceler.Tbl_Sehirler.SehirAdi,
                                 odaFiyatNormal = i.Tbl_Odalar.Where(s => s.OdaTuruID == odaSinifiID).Select(s => s.NormalOdaFiyati).FirstOrDefault(),
                                 odaFiyatiIndirim = i.Tbl_Odalar.Where(s => s.OdaTuruID == odaSinifiID).Select(s => s.IndirimliOdaFiyati).FirstOrDefault(),
                                 odaResmi = i.Tbl_Odalar.Where(s => s.OdaTuruID == odaSinifiID).Select(s => s.OdaResmi).FirstOrDefault(),
                                 otelAdres = i.AcikAdres,
                                 otelYildizi = i.OtelYildizi,
                                 otelTelefonu = i.OtelTelefonu,
                                 kullaniciYildiziOrt = otelRatingOrtalama,
                                 odaSinifiSayisi = otelinOdaSinifiOdaSayisi,
                                 odaSinifiOzellik = odaSinifiOzellikleri
                             }).ToList();
            ViewBag.OdaSayisi = otelinOdaSinifiOdaSayisi;
            var otelYorumlari = op.OtelYorumListesi(otelid);

            List<Models.OtelInceleme> otelIncelemesi = new List<Models.OtelInceleme>();
            foreach (var otelincele in otelDetay)
            {
                otelIncelemesi.Add(new Models.OtelInceleme()
                {
                    OtelID = otelincele.otelID,
                    OtelAdi = otelincele.otelAdi,
                    OtelAciklamasi = otelincele.otelAciklamasi,
                    OtelIlcesi = otelincele.otelIlcesi,
                    OtelSehri = otelincele.otelSehri,
                    NormalOdaFiyati = (decimal)otelincele.odaFiyatNormal,
                    IndirimliOdaFiyati = (decimal)otelincele.odaFiyatiIndirim,
                    KisiSayisi = kisiSayisi,
                    OdaResmi = otelincele.odaResmi,
                    OtelAdresi = otelincele.otelAdres,
                    OtelYildizi = int.Parse(otelincele.otelYildizi),
                    OtelRating = otelincele.kullaniciYildiziOrt,
                    OtelTelefonu = otelincele.otelTelefonu,
                    OtelYorumlari = otelYorumlari,
                    OtelinTureGoreOdaSayisi = otelincele.odaSinifiSayisi,
                    OdaSinifiOzellikleri = otelincele.odaSinifiOzellik
                });
            }
            return View(otelIncelemesi);
        }
        public ActionResult RezervasyonYap()
        {
            if (Session["LoginUser"] == null)
                return RedirectToAction("KullaniciGirisi", "Login");
            else
            {
                int kullaniciID = ((Tbl_Kullanicilar)Session["LoginUser"]).KullaniciID;
                int otelid = (int)Session["HotelID"];
                int odaSinifiID = ((List<Models.OtelAramaSonuclari>)Session["HotelResult"]).First().OdaSinifiID;
                DateTime girisTarihi = ((List<Models.OtelAramaSonuclari>)Session["HotelResult"]).First().girisTarihi;
                DateTime cikisTarihi = ((List<Models.OtelAramaSonuclari>)Session["HotelResult"]).First().cikisTarihi;
                int geceSayisi = (cikisTarihi - girisTarihi).Days;
                int kisiSayisi = ((List<Models.OtelAramaSonuclari>)Session["HotelResult"]).First().kisiSayisi;
                Tbl_Odalar rezerveOda = (from i in db.Tbl_Odalar where i.Tbl_Oteller.OtelID == otelid && i.OdaTuruID == odaSinifiID && i.IsActive.Value select i).First();
                int odaID = rezerveOda.OdaID;
                decimal odenecekTutar = (decimal)rezerveOda.IndirimliOdaFiyati * geceSayisi;
                op.RezervasyonYap(kullaniciID, odaID, kisiSayisi, odenecekTutar, girisTarihi, cikisTarihi);
                rezerveOda.IsActive = false;
                db.SaveChanges();

                //MAIL ATMA
                SmtpClient mail = new SmtpClient("smtp-mail.outlook.com", 587);
                mail.Timeout = 10000;
                mail.DeliveryMethod = SmtpDeliveryMethod.Network;
                mail.UseDefaultCredentials = false;
                mail.Credentials = new NetworkCredential("skybooking@outlook.com.tr", "bookingproje123.");
                MailMessage mesaj = new MailMessage("skybooking@outlook.com.tr", "skybooking@outlook.com.tr");//Gmail sorun yaptığı için outlook'tan outlook'a gönderiliyor. Normalde session'dan gelen mail'e atması gerek.
                mesaj.IsBodyHtml = true;
                mesaj.Subject = "Rezervasyon Bilgileri - SkyBooking.Com";
                mesaj.Body = "Otel Adı: " + op.OtelAdi(otelid) + "</br>" + "Otel Adresi: " + op.OtelAdresi(otelid) + "</br>" + "Oda Numarası: " + rezerveOda.OdaNumarasi.ToString() + "</br>" + "Giriş Tarihi: " + girisTarihi.ToString("dd/MM/yyyy") + "</br>" + "Çıkış Tarihi: " + cikisTarihi.ToString("dd/MM/yyyy") + "</br>" + "Gece Sayısı: " + geceSayisi.ToString() + "</br>" + "Ödenen Tutar: " + string.Format("{0:C0}", odenecekTutar) + "</br><b>İyi tatiller dileriz.</b>";
                mail.EnableSsl = true;
                mesaj.BodyEncoding = Encoding.UTF8;
                mesaj.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                mail.Send(mesaj);


                return RedirectToAction("Rezervasyonlarim", "Home");
            }
        }
        public ActionResult Rezervasyonlarim()
        {
            if (Session["LoginUser"] == null)
                return RedirectToAction("KullaniciGirisi", "Login");
            else
            {
                int kullaniciID = ((Tbl_Kullanicilar)Session["LoginUser"]).KullaniciID;
                var rezBilgileri = (from i in db.Tbl_Rezervasyonlar
                                    where i.KullaniciID == kullaniciID
                                    select new
                                    {
                                        otelAdi = i.Tbl_Odalar.Tbl_Oteller.OtelAdi,
                                        otelAdresi = i.Tbl_Odalar.Tbl_Oteller.AcikAdres,
                                        otelTelefonu = i.Tbl_Odalar.Tbl_Oteller.OtelTelefonu,
                                        kisiSayisi = i.KisiSayisi,
                                        otelResmi = i.Tbl_Odalar.Tbl_Oteller.OtelResmi,
                                        tutar = i.OdenenTutar,
                                        odaNo = i.Tbl_Odalar.OdaNumarasi,
                                        girisTarihi = i.GirisTarihi,
                                        cikisTarihi = i.CikisTarihi
                                    }).ToList();
                List<Models.RezervasyonBilgileri> rezervasyonlar = new List<Models.RezervasyonBilgileri>();
                foreach (var rezervasyon in rezBilgileri)
                {
                    rezervasyonlar.Add(new Models.RezervasyonBilgileri()
                    {
                        OtelAdi = rezervasyon.otelAdi,
                        OtelAdresi = rezervasyon.otelAdresi,
                        OtelTelefonu = rezervasyon.otelTelefonu,
                        KisiSayisi = rezervasyon.kisiSayisi.Equals(null) ? 0 : (int)rezervasyon.kisiSayisi,
                        Tutar = rezervasyon.tutar.Equals(null) ? 0 : (decimal)rezervasyon.tutar,
                        OtelResmi = rezervasyon.otelResmi,
                        GirisTarihi = (DateTime)rezervasyon.girisTarihi,
                        CikisTarihi = (DateTime)rezervasyon.cikisTarihi,
                        OdaNumarasi = (int)rezervasyon.odaNo
                    });
                }
                return View(rezervasyonlar);
            }
        }
        [HttpPost]
        public ActionResult OteliIncele(FormCollection frm)
        {
            if (Session["LoginUser"] == null)
                return RedirectToAction("KullaniciGirisi", "Login");
            else
            {
                int userID = ((Tbl_Kullanicilar)Session["LoginUser"]).KullaniciID;
                int otelID = (int)Session["HotelID"];
                string yorum = frm["txtYorum"];
                int puan = int.Parse(frm["ddlPuanlama"]);
                op.OtelePuanVer(otelID, userID, puan);
                op.OteleYorumYap(otelID, userID, yorum);
                return RedirectToAction("OteliIncele", "Home", new { @otelid = otelID });
            }
        }
    }
}