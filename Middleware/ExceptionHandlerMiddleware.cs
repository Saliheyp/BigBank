using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MvcBigBank.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

using HttpContext = Microsoft.AspNetCore.Http.HttpContext;

namespace MvcBigBank.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IHostingEnvironment hostingEnvironment)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
               ExceptionHandlerMiddleware.Log(e);
            }
        }

        public static void Log( Exception exception)
        {
            try
            {
                db_BankProjectEntities1 db = new db_BankProjectEntities1();
                Models.ExceptionLog el = new Models.ExceptionLog();
                el.detailDescription = exception.Message;
                el.eventTime = DateTime.Now;
                db.ExceptionLog.Add(el);
                db.SaveChanges();
            }
            catch (Exception error)
            {

                ExceptionHandlerMiddleware.Log(error);
            }
           
           

          
        }
    }
}