using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.MP.Sample.CommonService.CustomMessageHandler;

namespace Senparc.Weixin.MP.Sample.Controllers
{
    public class HomeController : Controller
    {
        public const string WeixinToken = "";

        /// <summary>
        /// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML
        /// </summary>
        [HttpPost]
        [ActionName("Index")]
        public IActionResult Post(string signature, string timestamp, string nonce, string echostr)
        {
            return Content("");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
