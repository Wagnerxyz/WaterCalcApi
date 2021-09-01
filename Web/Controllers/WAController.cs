using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Http;
using System.Web.Mvc;
using LiaoDongBayTest;
using Models;
using WengAn.Args;

namespace LiaoDongBay.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class WAController : ApiController
    {
        object __lockObj = new object();
        private static bool isRunnning = false;
        private readonly string modelPath;
        private const string fileName = "WengAn20210813.wtg.sqlite";
        const string message = "前一个请求正在运行，请稍后再试";

        public WAController()
        {
            modelPath = @"C:\Data\WengAn\WengAn20210813\WengAn20210813.wtg.sqlite";
            //string path = ConfigurationManager.AppSettings["WengAnModelsFolder"];
            //modelPath = Path.Combine(path, fileName);

        }
        /// <summary>
        /// 运行水力模型
        /// </summary>
        /// <param name="modelPath"></param>
        /// <returns></returns>
        public IHttpActionResult RunEps(string modelPath)
        {
            if (isRunnning)
            {
                return BadRequest(message);
            }
            //override
            modelPath = this.modelPath;
            try
            {
                System.Threading.Monitor.Enter(__lockObj, ref isRunnning);
                var result = new WengAnEpsResult();
                result = WengAnApi.RunEPS(modelPath);
                return Ok(result);

            }
            finally
            {
                if (isRunnning)
                {
                    System.Threading.Monitor.Exit(__lockObj);
                    isRunnning = false;
                }
            }

        }
        /// <summary>
        /// 爆管影响分析
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public IHttpActionResult BreakPipe(BreakPipeArg arg)
        {
            if (isRunnning)
            {
                return BadRequest(message);
            }
            //override
            arg.ModelPath = this.modelPath;
            try
            {
                System.Threading.Monitor.Enter(__lockObj, ref isRunnning);
                var result = new BreakPipeResult();
                result = WengAnApi.BreakPipe(arg);
                return Ok(result);

            }
            finally
            {
                if (isRunnning)
                {
                    System.Threading.Monitor.Exit(__lockObj);
                    isRunnning = false;
                }
            }

        }
        /// <summary>
        /// 水源追踪
        /// </summary>
        /// <param name="modelPath"></param>
        /// <returns></returns>
        public IHttpActionResult WaterTrace(string modelPath)
        {
            if (isRunnning)
            {
                return BadRequest(message);
            }
            //override
            modelPath = this.modelPath;
            try
            {
                System.Threading.Monitor.Enter(__lockObj, ref isRunnning);
                var result = WengAnApi.GetWaterTraceResultsForMultipleElementIds(modelPath);
                return Ok(result);

            }
            finally
            {
                if (isRunnning)
                {
                    System.Threading.Monitor.Exit(__lockObj);
                    isRunnning = false;
                }
            }

        }
    }
}