using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingProje.Business;
using BookingProje.Data;

namespace BookingProje.UI.Controllers
{
    public class LoginController : Controller
    {
        BookingOperation op;
        public LoginController()
        {
            op = new BookingOperation();
        }
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult KullaniciGirisi()
        {
            if (Session["LoginUser"] != null)
                return RedirectToAction("Index", "Home");
            else
            return View();
        }
        [HttpPost]
        public ActionResult KullaniciGirisi(FormCollection frm)
        {
            if (string.IsNullOrEmpty(frm["txtEmail"]) || string.IsNullOrEmpty(frm["txtSifre"]))
            {
                TempData["msg"] = "<script>alert('Lütfen gerekli alanları doldurunuz.');</script>";
                return View();
            }
            else
            {
                if (!op.KullaniciVarMi(frm["txtEmail"], frm["txtSifre"]))
                {
                    TempData["msg"] = "<script>alert('Böyle bir kullanıcı bulunamadı.');</script>";
                    return View();
                }
                else
                {
                    Session["LoginUser"] = op.KullaniciGetir(frm["txtEmail"], frm["txtSifre"]);
                    return RedirectToAction("Index", "Home");
                }
            }
        }
        public ActionResult KullaniciKayit()
        {
            return View();
        }
        [HttpPost]
        public ActionResult KullaniciKayit(FormCollection frm)
        {
            if (op.KullaniciKaydiVarMi(frm["txtKullaniciEmail"], frm["txtKullaniciTelefon"]))
            {
                TempData["msg"] = "<script>alert('Böyle bir e-mail ya da telefon numarası zaten kayıtlı.');</script>";
                return View();
            }
            else
            {
                if (op.KullaniciKaydi(frm["txtKullaniciAd"], frm["txtKullaniciSoyad"], frm["txtKullaniciEmail"], frm["txtKullaniciSifre"], frm["txtKullaniciTelefon"]))
                {
                    TempData["msg"] = "<script>alert('Kayıt işlemi başarılı.');</script>";
                    return RedirectToAction("KullaniciGirisi", "Login");
                }
                else
                {
                    TempData["msg"] = "<script>alert('Lütfen gerekli alanları doldurunuz.');</script>";
                    return View();
                }
            }
        }
        public PartialViewResult KullaniciPaneli()
        {
            Tbl_Kullanicilar kullanici = (Tbl_Kullanicilar)Session["LoginUser"];
            return PartialView(kullanici);
        }
        public ActionResult GorevliGirisi()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GorevliGirisi(FormCollection frm)
        {
            if (string.IsNullOrEmpty(frm["txtEmail"]) || string.IsNullOrEmpty(frm["txtSifre"]) || string.IsNullOrEmpty(frm["txtOtelKodu"]))
            {
                TempData["msg"] = "<script>alert('Lütfen gerekli alanları doldurunuz.');</script>";
                return View();
            }
            else
            {
                if (!op.GorevliVarMi(frm["txtEmail"], frm["txtSifre"]))
                {
                    TempData["msg"] = "<script>alert('Böyle bir görevli bulunamadı.');</script>";
                    return View();
                }
                else
                {
                    Session["LoginEmployee"] = op.GorevliGetir(frm["txtEmail"], frm["txtSifre"], frm["txtOtelKodu"]);
                    Session["HotelCode"] = frm["txtOtelKodu"];
                    return RedirectToAction("Index", "OtelPaneli");
                }
            }
        }
        public ActionResult OtelBasvuru()
        {
            SehirlerveIlceler();
            return View();
        }
        [HttpPost]
        public ActionResult OtelBasvuru(FormCollection frm)
        {
            SehirlerveIlceler();
            if(op.OtelBasvurusu(frm["txtOtelAdi"], frm["txtAdres"], int.Parse(frm["ddlIlceler"]), int.Parse(frm["ddlYildizlar"]), int.Parse(frm["numbOdaSayisi"]), frm["txtGorevliAdi"], frm["txtGorevliSoyadi"], frm["txtGorevliEmail"], frm["txtGorevliSifre"], frm["txtGorevliTelefon"]))
            {
                TempData["msg"] = "<script>alert('Otel başvurunuz yöneticiye iletildi. Yanıtı mail adresinize göndereceğiz.');</script>";
            }
            else
            {
                TempData["msg"] = "<script>alert('Lütfen gerekli alanları doldurunuz.');</script>";
            }
            return View();
        }
        public ActionResult CikisYap()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
        public void SehirlerveIlceler()
        {
            SelectList sehirler = new SelectList(op.SehirListesi(), "SehirID", "SehirAdi");
            ViewBag.Sehirler = sehirler;
        }
    }
}