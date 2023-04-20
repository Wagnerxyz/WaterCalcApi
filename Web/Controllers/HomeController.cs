using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        /// <summary>
        /// 健康检查
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        public ActionResult HealthCheck()
        {
            //using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnection"].ToString()))
            //{
            //    try
            //    {
            //        connection.Open();
            //    }
            //    catch (SqlException)
            //    {
            //        return Content(HttpStatusCode.ServiceUnavailable, "服务失败");
            //    }
            //}
            return Content("服务健康");
        }
    }
}
