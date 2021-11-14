using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Microsoft.AspNetCore.Http;
using MvcBigBank.Middleware;
using MvcBigBank.Models;
namespace MvcBigBank.Controllers
{
    public class HomeController : Controller
    {
        private db_BankProjectEntities1 db = new db_BankProjectEntities1();
        static string temp = "";
        static decimal? toplam=0;
        static int counter = 0;
        static string hold=null;
        static string hold2= null;
        public void Info()
        {
            try
            {
                toplam = 0;
                string LoginTCKN = Session["LoginUser"].ToString();
                int id = 0;
                foreach (var item in db.Musteri)
                {
                    if (item.tcKimlik == LoginTCKN)
                    {
                        temp = item.ad + " " + item.soyad;
                        id = item.musteriID;
                        break;
                    }
                }

                foreach (var item in db.Hesap)
                {
                    if (item.musteriID == id)
                    {
                        toplam += item.bakiye;
                    }
                }
            }
            catch (Exception error)
            {
                ExceptionHandlerMiddleware.Log(error);
            }
           

        }
        public ActionResult Index()
        {
            try
            {
                counter++;
                this.Info();
                ViewBag.Message = temp.ToUpper();
                ViewBag.Message2 = "Başarıyla Giriş Yaptınız";
                ViewBag.Counter = counter;
                ViewBag.value = toplam;
            }
            catch (Exception error)
            {
                ExceptionHandlerMiddleware.Log(error);
            }
           
           // DateTime dateTime = Convert.ToDateTime("S2019 - 01 - 22 1:21 ÖS40");
            return View();
        }

        public ActionResult Login()
        {
            ViewBag.Message3 = hold;
            ViewBag.Message4 = hold2;
            hold = null;
            hold2= null;
            return View();
        }

        public ActionResult Update()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(string tcKimlik, string sifre)
        {

            try
            {
                sifre = SHA512Encoding(sifre);
                if (db.Musteri.Count(e => e.tcKimlik == tcKimlik && e.sifre == sifre) > 0) 
                {
                    Session.Add("LoginUser", tcKimlik);
                    this.Info();
                    this.Index();
                    return View("Index");
                }

                else
                {
                    ViewBag.Message = "Yanlış KullanıcıAdı veya Şifre";
                    return View("Login");
                }
            }
            catch (Exception error)
            {
                ExceptionHandlerMiddleware.Log(error);
                return View("Login");
            }
        }

        public ActionResult Register()
        {
            return View();
        }
        public string MusteriDogrulama(string tcKimlik)
        {
            try
            {
                if (db.Musteri.Count(e => e.tcKimlik == tcKimlik) > 0)
                    return tcKimlik;
                else
                    return null;
            }
            catch (Exception error)
            {
                ExceptionHandlerMiddleware.Log(error);
                return null;
            }
       
        }

        Random rastgele = new Random();


        [HttpPost]
        public ActionResult Register(Musteri model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Message4 = "Kayıt Başarısız Oldu !!!";
                    return View("Register");
                }
                else
                {
                    var _tckn = MusteriDogrulama(model.tcKimlik);
                    if (_tckn == null)
                    {
                        Random rastgele = new Random();
                       
                        model.musteriNo = Convert.ToString(rastgele.Next(100000000, 999999999));
                        model.sifre = SHA512Encoding(model.sifre);
                        db.Musteri.Add(model);
                        db.SaveChanges();
                        hold2 = "Kaydınız Basarıyla Oluşturulmuştur";
                        return RedirectToAction("Login", "Home");
                    }
                    else
                    {
                        hold2="TC Numaranıza Bankamıza Kayıtlıdır.";
                        return RedirectToAction("Login", "Home");
                    }

                }
            }
            catch (Exception error)
            {
                ExceptionHandlerMiddleware.Log(error);
                return View("Register");
            }
        }

        [HttpPost]
        public ActionResult Update(Musteri model)
        {
            
            string LoginTCKN = Session["LoginUser"].ToString();
            foreach (var item in db.Musteri)
            {
                if (item.tcKimlik == LoginTCKN)
                {
                    model.tcKimlik=LoginTCKN;
                    model.dogumTarihi = item.dogumTarihi;
                }
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Message3 = "...Güncelleme Başarısız Oldu...";
                    return View("Update");
                }
                else
                {
                    db.sp_update(model.tcKimlik,model.ad,model.soyad,model.sifre,model.dogumTarihi,model.telefon,model.eMail,model.adres);
                    toplam = 0;
                    hold = "Bilgileriniz Başarıyla Guncellenmistir";
                    return RedirectToAction("LogOut", "Home");
                }
            }
            catch (Exception error)
            {
                ModelState.AddModelError("", error.Message);
                return View("Update");
            }
        }

        
        public ActionResult LogOut()
        {

            toplam = 0;
            counter = 0;
            return RedirectToAction("Login", "Home");

        }
        public string SHA512Encoding( string sifre)
        {
            try
            {
                SHA512Managed sha512 = new SHA512Managed();
                byte[] bitDizisi = System.Text.Encoding.UTF8.GetBytes(sifre);
                string sifreliVeri = Convert.ToBase64String(sha512.ComputeHash(bitDizisi));
                return sifreliVeri;
            }
            catch (Exception error)
            {
                ExceptionHandlerMiddleware.Log(error);
                return null;
            }
    
        }
    }
}