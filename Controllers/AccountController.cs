using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcBigBank.Middleware;
using MvcBigBank.Models;

namespace MvcBigBank.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {
        HomeController homeController = new HomeController();
        private db_BankProjectEntities1 db = new db_BankProjectEntities1();
        static int globalIdHolder = 0;
        static string hold = null;
        static string hold2 = null;
        static string hold3 = null;
        static string hold4 = null;
        static string hold5 = null;
        static string hold6 = null;
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View();
        }

        public ActionResult AccountDetails()
        {
            try
            {
                ViewBag.Message2 = hold;
                ViewBag.Message3 = hold2;
                ViewBag.Message4 = hold3;
                ViewBag.Message5 = hold4;
                ViewBag.Message6 = hold5;
                ViewBag.Message7 = hold6;
            }
            catch (Exception error)
            {
                ExceptionHandlerMiddleware.Log(error);
            }
            
            return View();
        }

        public ActionResult DepositMoney(int id)
        {
            globalIdHolder = id;
            return View();
        }

        public ActionResult WithdrawMoney(int id)
        {
            try
            {
                foreach (var item in db.Hesap)
                {
                    if (item.hesapID == id)
                    {
                        globalIdHolder = id;
                        ViewBag.value = item.bakiye;
                    }
                }
            }
            catch (Exception errror)
            {

                ExceptionHandlerMiddleware.Log(errror);
            }


            return View();
        }

        public ActionResult MoneyTransfer(int id)
        {
            try
            {
                globalIdHolder = id;

            }
            catch (Exception error)
            {
                ExceptionHandlerMiddleware.Log(error);
            }
            return View();
        }

        public ActionResult Virman(int id)
        {
            try
            {
                globalIdHolder = id;
                int temp = 0;
                List<Hesap> list = new List<Hesap>();

                foreach (var item in db.Hesap)
                {
                    if (item.hesapID == id)
                    {
                        temp = item.musteriID;
                        break;
                    }
                }

                foreach (var item in db.Hesap)
                {
                    if (item.musteriID == temp && item.aktiflik == true && item.hesapID != id)
                    {
                        list.Add(item);
                    }
                }

                ViewBag.list = list;
            }
            catch (Exception error)
            {
                ExceptionHandlerMiddleware.Log(error);
            }
            return View();
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult MoneyTransfer(string aliciHesapNo, decimal miktar)
        {
            string temp = "";
            Hesap model = new Hesap();

            try
            {
                foreach (var item in db.Hesap)
                {
                    if (item.hesapID == globalIdHolder)
                    {
                        temp = item.hesapNo;
                        model = item;
                    }
                }
                if (model.bakiye < 0)
                {
                    throw new Exception("Hesap Bakiyeniz 0'dan az olamaz yeni bir tutar giriniz!");
                }
                else if (model.bakiye >= miktar)
                {
                    db.sp_moneyTransfer(temp, aliciHesapNo, miktar);
                    hold = "İşleminiz Başarıyla Gerçekleşmiştir";
                    return RedirectToAction("AccountDetails", "Account");
                }
                else
                {
                    hold = "Girilen tutar hesabınızdaki para miktarından fazla!";
                    return RedirectToAction("AccountDetails", "Account");

                }


            }
            catch (Exception error)
            {
                ExceptionHandlerMiddleware.Log(error);
                return RedirectToAction("Index", "Home");
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Virman(string aliciHesapNo, decimal miktar)
        {
            string temp = "";
            Hesap model = new Hesap();

            try
            {
                foreach (var item in db.Hesap)
                {
                    if (item.hesapID == globalIdHolder)
                    {
                        temp = item.hesapNo;
                        model = item;
                    }
                }
                if (model.bakiye < 0)
                {
                    throw new Exception("Hesap Bakiyeniz 0'dan az olamaz yeni bir tutar giriniz!");
                }
                else if (model.bakiye >= miktar)
                {
                    db.sp_moneyTransfer(temp, aliciHesapNo, miktar);
                    hold2 = "İşleminiz Başarıyla Gerçekleşmiştir";
                    return RedirectToAction("AccountDetails", "Account");
                }
                else
                {
                    hold2 = "Girilen tutar hesabınızdaki para miktarından fazla!";
                    return RedirectToAction("AccountDetails", "Account");

                }


            }
            catch (Exception error)
            {
                ExceptionHandlerMiddleware.Log(error);
                return RedirectToAction("Index", "Home");
            }
        }

        public int MusteriIdAlma(string tcKimlik)
        {
            try
            {
                int temp = 0;
                foreach (var item in db.Musteri)
                {
                    if (item.tcKimlik == tcKimlik)
                        temp = item.musteriID;
                }
                return temp;
            }
            catch (Exception error)
            {
                ExceptionHandlerMiddleware.Log(error);
                return 0;
            }

        }
        private int HesapSayisi(int id)
        {
            try
            {
                int temp = 0;
                temp = db.Hesap.Count(e => e.musteriID == id);
                return temp;

            }
            catch (Exception error)
            {
                ExceptionHandlerMiddleware.Log(error);
                return 0;
            }
            
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateAccount(decimal bakiye)
        {
            try
            {
                string LoginTCKN = Session["LoginUser"].ToString();

                int ekNo = 1001;
                int temp = MusteriIdAlma(LoginTCKN);
                int hesapSayisi = HesapSayisi(temp);
                if (hesapSayisi == 0)
                    ekNo = 1001;
                else if (hesapSayisi > 0)
                {
                    ekNo += hesapSayisi;
                }
                db.sp_HesapOlustur(LoginTCKN, bakiye, ekNo);
                hold3 = "İşleminiz Başarıyla Gerçekleşmiştir";
                return RedirectToAction("AccountDetails", "Account");
            }
            catch (Exception error)
            {
                ExceptionHandlerMiddleware.Log(error);
                return RedirectToAction("AccountDetails", "Account");

            }


        }


        [HttpGet]
        public ActionResult DeleteAccount(int id)
        {
            try
            {
                string LoginTCKN = Session["LoginUser"].ToString();
                Hesap hesap = new Hesap();
                foreach (var item in db.Hesap)
                {
                    if (item.hesapID == id)
                        hesap = item;

                }
                if (ModelState.IsValid)
                {

                    if (hesap.bakiye == 0)
                    {
                        hesap.musteriID = MusteriIdAlma(LoginTCKN);
                        db.sp_Deactivate(id);
                        hold4 = "Hesabınız Kapatılmıştır";
                        return RedirectToAction("AccountDetails", "Account");
                    }
                    else
                    {
                        hold4 = "Hesabınızı silmek için lütfen bakiyenizi sıfırlayınız";
                        return RedirectToAction("AccountDetails", "Account");
                    }
                }
                else
                {
                    hold4 = "Eksik Bilgi Girildi.. Tekrar Deneyiniz..";
                    return RedirectToAction("AccountDetails", "Account");

                }
            }
            catch (Exception error)
            {
                ExceptionHandlerMiddleware.Log(error);
                return RedirectToAction("AccountDetails", "Account");

            }
        }

        private List<Hesap> HesapLister(int id)
        {
            try
            {
                List<Hesap> temp = new List<Hesap>();
                foreach (var item in db.Hesap)
                {
                    if (item.musteriID == id)
                        temp.Add(item);
                }
                return temp;
            }
            catch (Exception error)
            {
                ExceptionHandlerMiddleware.Log(error);
                return null;
            }

        }

        public int HesapIdAlma(string hesapNo)
        {
            try
            {
                int temp = 0;
                foreach (var item in db.Hesap)
                {
                    if (item.hesapNo == hesapNo)
                        temp = item.hesapID;
                }
                return temp;
            }
            catch (Exception error)
            {
                ExceptionHandlerMiddleware.Log(error);
                return 0;
            }

        }

        private string HesapNoAlma(int id)
        {
            try
            {
                string temp = "";
                foreach (var item in db.Hesap)
                {
                    if (item.musteriID == id)
                        temp = item.hesapNo;
                }
                return temp;
            }
            catch (Exception error)
            {
                ExceptionHandlerMiddleware.Log(error);
                return null;

            }

        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult WithdrawMoney(decimal bakiye)
        {
            Hesap model = new Hesap();
            decimal temp = bakiye * (-1);

            try
            {
                foreach (var item in db.Hesap)
                {
                    if (item.hesapID == globalIdHolder)
                    {
                        model = item;
                    }
                }

                if (model != null)
                {
                    if (model.bakiye < 0)
                    {
                        throw new Exception("Hesap Bakiyeniz 0'dan az olamaz yeni bir tutar giriniz!");
                    }
                    else if (model.bakiye < bakiye)
                    {
                        hold5 = "Hesap Bakiyesi yetersiz..";
                        return RedirectToAction("AccountDetails", "Account");

                    }
                    else
                    {
                        db.sp_moneyDrawInOut(model.hesapNo, temp);
                        hold5 = "Paranız Tahsil Edildi";
                        return RedirectToAction("AccountDetails", "Account");
                    }
                }
                else
                {
                    hold5 = "Boyle Bir Hesap Bulunamadı";
                    return RedirectToAction("AccountDetails", "Account");

                }

            }

            catch (Exception error)
            {
                ExceptionHandlerMiddleware.Log(error);
                return RedirectToAction("Index", "Home");
            }

        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DepositMoney(decimal bakiye)
        {
            Hesap model = new Hesap();
            try
            {
                foreach (var item in db.Hesap)
                {
                    if (item.hesapID == globalIdHolder)
                    {
                        model = item;
                    }
                }

                if (model != null)
                {
                    if (model.bakiye < 0)
                    {
                        throw new Exception("Hesap Bakiyeniz 0'dan az olamaz yeni bir tutar giriniz!");
                    }

                    else
                    {
                        db.sp_moneyDrawInOut(model.hesapNo, bakiye);
                        hold5 = "Paranız Yatırılıyor";
                        return RedirectToAction("AccountDetails", "Account");
                    }
                }
                else
                {
                    hold5 = "Boyle Bir Hesap Bulunamadı";
                    return RedirectToAction("AccountDetails", "Account");
                }

            }

            catch (Exception error)
            {
                ExceptionHandlerMiddleware.Log(error);
                return RedirectToAction("Index", "Home");
            }

        }


    }
}