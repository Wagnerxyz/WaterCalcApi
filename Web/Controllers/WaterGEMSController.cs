using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Web.Controllers
{
    /// <summary>
    /// 甘薯中建
    /// </summary>
    public class WaterGEMSController : ApiController
    {
        /// <summary>
        /// 需水量预测
        /// </summary>
        /// <returns></returns>
        /// <response code="200">正常结果</response>
        public IHttpActionResult ForecastDemand()
        {
           return Ok();
        }
        /// <summary>
        /// 修改模型需水量参数
        /// </summary>
        /// <returns></returns>
        /// <response code="200">正常结果</response>
        public IHttpActionResult UpdateDemand()
        {
            return Ok();
        }
        /// <summary>
        /// 修改模型泵阀操作
        /// </summary>
        /// <returns></returns>
        /// <response code="200">修改模型泵阀操作</response>
        public IHttpActionResult UpdatePumpValve()
        {
            return Ok();
        }
        /// <summary>
        /// 运行供水管网模型
        /// </summary>
        /// <returns></returns>
        /// <response code="200">正常结果</response>
        public IHttpActionResult RunCalculation()
        {
            return Ok();
        }

        /// <summary>
        /// 读取计算结果
        /// </summary>
        /// <returns></returns>
        /// <response code="200">正常结果</response>
        [HttpGet]
        public IHttpActionResult ReadResult()
        {
            return Ok();
        }

        /// <summary>
        /// 启动达尔文漏失定位工具
        /// </summary>
        /// <returns></returns>
        /// <response code="200">正常结果</response>
        public IHttpActionResult RunDarwinTool()
        {
            return Ok();
        }
    }
}