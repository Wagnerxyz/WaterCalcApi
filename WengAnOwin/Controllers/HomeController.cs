using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace WengAnOwin.Controllers
{
    public class HomeController : ApiController
    {
        /// <summary>
        /// 健康检查
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult HealthCheck()
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

            return Ok("服务健康!");
        }
    }
}
