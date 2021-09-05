using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Http;
using System.Web.Http.Description;
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
        private object __lockObj = new object();
        private static bool isRunnning = false;
        private readonly string modelPath;
        const string message = "前一个请求正在运行，请稍后再试";

        public WAController()
        {
            modelPath = @"D:\BentleyModels\WengAn\WengAn20210813\WengAn20210813.wtg.sqlite";
            //string path = ConfigurationManager.AppSettings["WengAnModelsFolder"];
            //modelPath = Path.Combine(path, fileName);

        }
        /// <summary>
        /// 运行水力模型
        /// </summary>
        /// <remarks>
        /// POST /ApiTest/api/WA/WaterTrace?modelpath=555
        /// </remarks>
        /// <param name="modelPath">模型路径</param>
        /// <returns></returns>
        [ResponseType(typeof(WengAnEpsResult))]

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
        /// <remarks>
        /// 模型缺少阀门，无法计算
        ///
        /// </remarks>
        /// <param name="arg">输入参数</param>
        /// 
        /// <returns></returns>
        [ResponseType(typeof(BreakPipeResult))]
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
        /// <param name="modelPath">模型路径</param>
        /// <returns></returns>
        [ResponseType(typeof(List<WaterTraceResult>))]
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
                List<WaterTraceResult> result = WengAnApi.GetWaterTraceResultsForMultipleElementIds(modelPath);
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