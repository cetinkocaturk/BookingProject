using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingProje.Data;
using BookingProje.Business;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace BookingProje.UI.Controllers
{
    public class OtelPaneliController : Controller
    {
        BookingProjectEntities db;
        HotelOperation ho;
        public OtelPaneliController()
        {
            db = new BookingProjectEntities();
            ho = new HotelOperation();
            SelectList odaTuruListe = new SelectList(ho.OdaTuruListesi(), "OdaTuruID", "OdaTuru");
            ViewBag.OdaTurleri = odaTuruListe;
        }
        // GET: OtelPaneli
        public ActionResult Index()
        {
            if (Session["LoginEmployee"] == null)
            {
                return RedirectToAction("GorevliGirisi", "Login");
            }
            else
            {
                string otelKodu = (string)Session["HotelCode"];

                int gorevliID = ((Tbl_Gorevliler)Session["LoginEmployee"]).GorevliID;

                int otelID = ho.OtelGetir(gorevliID, otelKodu).OtelID;

                int? musteriSayisi = ho.MusteriSayisi(otelID);

                int bosOdaSayisi = ho.BosOdaSayisi(otelID);

                int doluOdaSayisi = ho.DoluOdaSayisi(otelID);

                var otelIstatistikleri = (from i in db.Tbl_Oteller
                                              //join x in db.Tbl_Odalar on i.OtelID equals x.OtelID
                                          where i.GorevliID == gorevliID && i.OtelKodu == otelKodu && i.OtelID == otelID && i.IsActive.Value
                                          select new
                                          {
                                              gadi = i.Tbl_Gorevliler.GorevliAdi,
                                              gsoyadi = i.Tbl_Gorevliler.GorevliSoyadi,
                                              msayisi = musteriSayisi,
                                              bosodasayisi = bosOdaSayisi,
                                              doluodasayisi = doluOdaSayisi,
                                              toplamodasayisi = (bosOdaSayisi + doluOdaSayisi)
                                          }).ToList();
                List<Models.OtelIstatistik> model = new List<Models.OtelIstatistik>();
                foreach (var myOtel in otelIstatistikleri)
                {
                    model.Add(new Models.OtelIstatistik()
                    {
                        GorevliAdi = myOtel.gadi,
                        GorevliSoyadi = myOtel.gsoyadi,
                        MusteriSayisi = (int)myOtel.msayisi,
                        BosOdaSayisi = myOtel.bosodasayisi,
                        DoluOdaSayisi = myOtel.doluodasayisi,
                        ToplamOdaSayisi = myOtel.toplamodasayisi
                    });
                }
                return View(model);
            }
        }
        public PartialViewResult GorevliBilgileri()
        {
            return PartialView((Tbl_Gorevliler)Session["LoginEmployee"]);
        }
        public ActionResult CikisYap()
        {
            Session.Abandon();
            return RedirectToAction("GorevliGirisi", "Login");
        }
        public ActionResult OtelBilgileri()
        {
            return View();
        }
        public ActionResult OtelBilgileriGuncelle()
        {
            if (Session["LoginEmployee"] == null)
            {
                return RedirectToAction("GorevliGirisi", "Login");
            }
            else
            {
                return View(ho.OtelGetir(((Tbl_Gorevliler)Session["LoginEmployee"]).GorevliID, (string)Session["HotelCode"]));
            }
        }
        [HttpPost]
        public ActionResult OtelBilgileriGuncelle(string txtOtelAdi, string txtAciklama, string txtTelNo, string txtOtelYildizi, HttpPostedFileBase fileOtelResmi, string txtAdres)
        {
            string otelKodu = (string)Session["HotelCode"];
            int gorevliID = ((Tbl_Gorevliler)Session["LoginEmployee"]).GorevliID;
            Tbl_Oteller guncellenecekOtel = (from i in db.Tbl_Oteller where i.OtelKodu == otelKodu && i.GorevliID == gorevliID select i).First();
            guncellenecekOtel.OtelAdi = txtOtelAdi;
            guncellenecekOtel.OtelAciklamasi = txtAciklama;
            guncellenecekOtel.OtelTelefonu = txtTelNo;
            guncellenecekOtel.OtelYildizi = txtOtelYildizi;
            guncellenecekOtel.AcikAdres = txtAdres;
            if (fileOtelResmi != null)
            {
                guncellenecekOtel.OtelResmi = new byte[fileOtelResmi.ContentLength];
                fileOtelResmi.InputStream.Read(guncellenecekOtel.OtelResmi, 0, fileOtelResmi.ContentLength);
            }
            db.SaveChanges();
            TempData["msg"] = "<script>alert('Otel bilgileriniz güncellendi.');</script>";
            return View(guncellenecekOtel);
        }
        public ActionResult OtelOdalari()
        {
            string otelKodu = (string)Session["HotelCode"];
            int gorevliID = ((Tbl_Gorevliler)Session["LoginEmployee"]).GorevliID;
            int otelID = ho.OtelGetir(gorevliID, otelKodu).OtelID;
            return View(ho.OdalariGetir(otelID, otelKodu));
        }
        public ActionResult OdaGuncelle(int odaid)
        {
            return View(ho.TekliOdaGetir(odaid));
        }
        [HttpPost]
        public ActionResult OdaGuncelle(string txtOdaID, string txtNormalFiyat, string txtIndirimliFiyat, HttpPostedFileBase fileOdaResmi)
        {
            int odaID = int.Parse(txtOdaID);
            Tbl_Odalar guncellenecekOda = (from i in db.Tbl_Odalar where i.OdaID == odaID select i).First();
            guncellenecekOda.NormalOdaFiyati = decimal.Parse(txtNormalFiyat);
            guncellenecekOda.IndirimliOdaFiyati = decimal.Parse(txtIndirimliFiyat);
            if (fileOdaResmi != null)
            {
                guncellenecekOda.OdaResmi = new byte[fileOdaResmi.ContentLength];
                fileOdaResmi.InputStream.Read(guncellenecekOda.OdaResmi, 0, fileOdaResmi.ContentLength);
            }
            db.SaveChanges();
            return RedirectToAction("OtelOdalari", "OtelPaneli");
        }
        public ActionResult OdaEkle()
        {
            if (Session["LoginEmployee"] == null)
                return RedirectToAction("GorevliGirisi", "Login");
            else
                return View();
        }
        [HttpPost]
        public ActionResult OdaEkle(FormCollection frm, HttpPostedFileBase fileOdaResmi)
        {
            if (Session["LoginEmployee"] == null)
                return RedirectToAction("GorevliGirisi", "Login");
            else
            {
                if (string.IsNullOrEmpty(frm["ddlOdaTurleri"]) || string.IsNullOrEmpty(frm["numbOdaNumarasi"]) || string.IsNullOrEmpty(frm["numbNormalFiyat"]) || string.IsNullOrEmpty(frm["numbIndirimliFiyat"]) || fileOdaResmi == null)
                {
                    TempData["msg"] = "<script>alert('Lütfen boş alanları doldurun.');</script>";
                    return View();
                }
                else
                {
                    string otelKodu = (string)Session["HotelCode"];
                    int gorevliID = ((Tbl_Gorevliler)Session["LoginEmployee"]).GorevliID;
                    int otelID = ho.OtelGetir(gorevliID, otelKodu).OtelID;
                    byte[] odaresmi = new byte[fileOdaResmi.ContentLength];
                    fileOdaResmi.InputStream.Read(odaresmi, 0, fileOdaResmi.ContentLength);

                    int eklendiMi = ho.OdaEkle(otelID, int.Parse(frm["ddlOdaTurleri"]), int.Parse(frm["numbOdaNumarasi"]), decimal.Parse(frm["numbNormalFiyat"]), decimal.Parse(frm["numbIndirimliFiyat"]), odaresmi);
                    if (eklendiMi > 0)
                    {
                        TempData["msg"] = "<script>alert('Oda başarıyla eklendi.');</script>";
                    }
                    else
                    {
                        TempData["msg"] = "<script>alert('Bir hata oluştu.');</script>";
                    }
                    return View();
                }
            }
        }
        public ActionResult OtelYorumlari()
        {
            string otelKodu = (string)Session["HotelCode"];
            int gorevliID = ((Tbl_Gorevliler)Session["LoginEmployee"]).GorevliID;
            int otelID = ho.OtelGetir(gorevliID, otelKodu).OtelID;
            var otelYorum = (from i in db.Tbl_Yorumlar
                             where i.OtelID == otelID && i.IsActive.Value
                             select new
                             {
                                 Ad = i.Tbl_Kullanicilar.KullaniciAdi,
                                 Soyad = i.Tbl_Kullanicilar.KullaniciSoyadi,
                                 Email = i.Tbl_Kullanicilar.KullaniciEmail,
                                 Telefon = i.Tbl_Kullanicilar.KullaniciTelefon,
                                 AtilanYorum = i.Yorum,
                                 VerilenPuan = i.Tbl_Kullanicilar.Tbl_Puanlamalar.FirstOrDefault().Puan
                             }).ToList();
            List<Models.YorumVePuan> model = new List<Models.YorumVePuan>();
            foreach (var yorumlar in otelYorum)
            {
                model.Add(new Models.YorumVePuan()
                {
                    Ad = yorumlar.Ad,
                    Soyad = yorumlar.Soyad,
                    Email = yorumlar.Email,
                    Telefon = yorumlar.Telefon,
                    Yorum = yorumlar.AtilanYorum,
                    Puan = yorumlar.VerilenPuan.Equals(null) ? 0 : (int)yorumlar.VerilenPuan
                });
            }
            return View(model);
        }
        public ActionResult AdmineMailGonderme()
        {
            if (Session["LoginEmployee"] == null)
                return RedirectToAction("GorevliGirisi", "Login");
            else
                return View();
        }
        [HttpPost]
        public ActionResult AdmineMailGonderme(FormCollection frm)
        {
            if (frm["txtKonu"].ToString() == null || frm["txtMesaj"].ToString() == null)
            {
                TempData["msg"] = "<script>alert('Boş alanları doldurunuz.');</script>";
                return View();
            }
            else
            {
                SmtpClient mail = new SmtpClient("smtp-mail.outlook.com", 587);
                mail.Timeout = 10000;
                mail.DeliveryMethod = SmtpDeliveryMethod.Network;
                mail.UseDefaultCredentials = false;
                mail.Credentials = new NetworkCredential("skybooking@outlook.com.tr", "bookingproje123.");
                MailMessage mesaj = new MailMessage("skybooking@outlook.com.tr", "skybooking@outlook.com.tr");
                mesaj.IsBodyHtml = true;
                mesaj.Subject = frm["txtKonu"];
                mesaj.Body = frm["txtMesaj"];
                mail.EnableSsl = true;
                mesaj.BodyEncoding = Encoding.UTF8;
                mesaj.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                mail.Send(mesaj);
                TempData["msg"] = "<script>alert('Talebiniz yöneticiye iletildi.');</script>";
                return View();
            }
        }
        public ActionResult OdaSil(int odaid)
        {
            if (Session["LoginEmployee"] == null)
                return RedirectToAction("GorevliGirisi", "Login");
            else
            {
                string odaSil = ho.OdaSil(odaid);
                TempData["msg"] = "<script>alert('" + odaSil + "');</script>";
                return RedirectToAction("OtelOdalari", "OtelPaneli");
            }
        }
    }
}