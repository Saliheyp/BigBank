using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcBigBank.Models;
using System.Net.Http;
using System.Threading.Tasks;
using RestSharp;
using System.Xml;
using System.Net;
using MvcBigBank.Middleware;

namespace MvcBigBank.Controllers
{
    public class CurrencyController : Controller
    {
        private db_BankProjectEntities1 db = new db_BankProjectEntities1();
        private static readonly HttpClient client = new HttpClient();


        public ActionResult Index()
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


                XmlDocument xml = new XmlDocument(); // yeni bir XML dökümü oluşturuyoruz.
                xml.Load("http://www.tcmb.gov.tr/kurlar/today.xml"); // bağlantı kuruyoruz.
                var Tarih_Date_Nodes = xml.SelectSingleNode("//Tarih_Date"); // Count değerini olmak için ana boğumu seçiyoruz.
                var CurrencyNodes = Tarih_Date_Nodes.SelectNodes("//Currency"); // ana boğum altındaki kur boğumunu seçiyoruz.
                int CurrencyLength = CurrencyNodes.Count; // toplam kur boğumu sayısını elde ediyor ve for döngüsünde kullanıyoruz.

                List<_Doviz> dovizler = new List<_Doviz>(); // Aşağıda oluşturduğum public class ile bir List oluşturuyoruz.
                for (int i = 0; i < CurrencyLength; i++) // for u çalıştırıyoruz.
                {
                    var cn = CurrencyNodes[i]; // kur boğumunu alıyoruz.
                                               // Listeye kur bilgirini ekliyoruz.
                    dovizler.Add(new _Doviz
                    {
                        Kod = cn.Attributes["Kod"].Value,
                        CrossOrder = cn.Attributes["CrossOrder"].Value,
                        CurrencyCode = cn.Attributes["CurrencyCode"].Value,
                        Unit = cn.ChildNodes[0].InnerXml,
                        Isim = cn.ChildNodes[1].InnerXml,
                        CurrencyName = cn.ChildNodes[2].InnerXml,
                        ForexBuying = cn.ChildNodes[3].InnerXml,
                        ForexSelling = cn.ChildNodes[4].InnerXml,
                        BanknoteBuying = cn.ChildNodes[5].InnerXml,
                        BanknoteSelling = cn.ChildNodes[6].InnerXml,
                        CrossRateOther = cn.ChildNodes[8].InnerXml,
                        CrossRateUSD = cn.ChildNodes[7].InnerXml,
                    });
                }
                var a = 100;
                var b = 50;
                a = 0;
                var x = b / a;
                ViewData["dovizler"] = dovizler; // dovizler List değerini data ya atıyoruz ön tarafta viewbag ile çekeceğiz.
            }
            catch (Exception error )
            {
                ExceptionHandlerMiddleware.Log(error);
            }
            
            return View();
        }
        public class _Doviz
        {
            public string CrossOrder { get; set; }
            public string Kod { get; set; }
            public string CurrencyCode { get; set; }
            public string Unit { get; set; }
            public string Isim { get; set; }
            public string CurrencyName { get; set; }
            public string ForexBuying { get; set; }
            public string ForexSelling { get; set; }
            public string BanknoteBuying { get; set; }
            public string BanknoteSelling { get; set; }
            public string CrossRateUSD { get; set; }
            public string CrossRateOther { get; set; }
        }

        public async Task<string> callAsyncCurrencyAsync()
        {
            var responseString = await client.GetStringAsync("https://www.tcmb.gov.tr/kurlar/today.xml");
            return responseString;
        }
    }
}