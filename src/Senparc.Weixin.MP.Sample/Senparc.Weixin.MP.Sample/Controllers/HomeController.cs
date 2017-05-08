using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.Sample.CommonService.CustomMessageHandler;
using System.Text;

namespace Senparc.Weixin.MP.Sample.Controllers
{
    public class HomeController : Controller
    {
        public const string Token = "weixin";
        public const string EncodingAESKey = "";

        readonly Func<string> _getRandomFileName = () => DateTime.Now.ToString("yyyyMMdd-HHmmss") + Guid.NewGuid().ToString("n").Substring(0, 6);

        /// <summary>
        /// 微信后台验证地址（使用Get），微信后台的“接口配置信息”的Url填写如：http://sdk.weixin.senparc.com/weixin
        /// </summary>
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(PostModel postModel, string echostr)
        {
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + postModel.Signature + "," + MP.CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, Token) + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }

        /// <summary>
        /// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
        /// </summary>
        [HttpPost]
        [ActionName("Index")]
        public ActionResult Post(PostModel postModel)
        {

            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return Content("参数错误！");
            }
            #region 打包 PostModel 信息

            postModel.Token = Token;//根据自己后台的设置保持一致
            postModel.EncodingAESKey = EncodingAESKey;//根据自己后台的设置保持一致
            postModel.AppId = Startup.SenparcWeixin.WeixinAppId;//根据自己后台的设置保持一致

            #endregion

            var maxRecordCount = 10;

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。

            string body = new StreamReader(Request.Body).ReadToEnd();           // log body

            byte[] requestData = Encoding.UTF8.GetBytes(body);
            Stream inputstream = new MemoryStream(requestData);

            var messageHandler = new CustomMessageHandler(inputstream, postModel, maxRecordCount);

            try
            {

                #region 记录 Request 日志

                var logPath = string.Format("{0}/App_Data/MP/{1}/", Startup.RootPath, DateTime.Now.ToString("yyyy-MM-dd"));
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }

                //测试时可开启此记录，帮助跟踪数据，使用前请确保App_Data文件夹存在，且有读写权限。    
                using (var fs = new FileStream(Path.Combine(logPath, string.Format("{0}_Request_{1}.txt", _getRandomFileName(), messageHandler.RequestMessage.FromUserName)), FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    messageHandler.RequestDocument.Save(fs);
                }

                if (messageHandler.UsingEcryptMessage)
                {
                    using (var fs = new FileStream(Path.Combine(logPath, string.Format("{0}_Request_Ecrypt_{1}.txt", _getRandomFileName(), messageHandler.RequestMessage.FromUserName)), FileMode.CreateNew, FileAccess.ReadWrite))
                    {
                        messageHandler.EcryptRequestDocument.Save(fs);
                    }
                }

                #endregion

                /* 如果需要添加消息去重功能，只需打开OmitRepeatedMessage功能，SDK会自动处理。
                 * 收到重复消息通常是因为微信服务器没有及时收到响应，会持续发送2-5条不等的相同内容的RequestMessage*/
                messageHandler.OmitRepeatedMessage = true;

                //执行微信处理过程
                messageHandler.Execute();


                #region 记录 Response 日志

                //测试时可开启，帮助跟踪数据

                //if (messageHandler.ResponseDocument == null)
                //{
                //    throw new Exception(messageHandler.RequestDocument.ToString());
                //}
                if (messageHandler.ResponseDocument != null)
                {
                    using (var fs = new FileStream(Path.Combine(logPath, string.Format("{0}_Response_{1}.txt", _getRandomFileName(), messageHandler.RequestMessage.FromUserName)), FileMode.CreateNew, FileAccess.ReadWrite))
                    {
                        messageHandler.ResponseDocument.Save(fs);
                    }
                }

                if (messageHandler.UsingEcryptMessage && messageHandler.FinalResponseDocument != null)
                {
                    //记录加密后的响应信息
                    using (var fs = new FileStream(Path.Combine(logPath, string.Format("{0}_Response_Final_{1}.txt", _getRandomFileName(), messageHandler.RequestMessage.FromUserName)), FileMode.CreateNew, FileAccess.ReadWrite))
                    {
                        messageHandler.FinalResponseDocument.Save(fs);
                    }
                }

                #endregion


                return Content(messageHandler.RequestDocument.ToString());
            }
            catch (Exception ex)
            {
                #region 异常处理
                WeixinTrace.Log("MessageHandler错误：{0}", ex.Message);


                var logPath = Startup.RootPath + "/App_Data/Error_" + _getRandomFileName() + ".txt";

                using (var fs = new FileStream(logPath, FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    using (TextWriter tw = new StreamWriter(fs))
                    {
                        tw.WriteLine("ExecptionMessage:" + ex.Message);
                        tw.WriteLine(ex.Source);
                        tw.WriteLine(ex.StackTrace);
                        //tw.WriteLine("InnerExecptionMessage:" + ex.InnerException.Message);

                        if (messageHandler.ResponseDocument != null)
                        {
                            tw.WriteLine(messageHandler.ResponseDocument.ToString());
                        }

                        if (ex.InnerException != null)
                        {
                            tw.WriteLine("========= InnerException =========");
                            tw.WriteLine(ex.InnerException.Message);
                            tw.WriteLine(ex.InnerException.Source);
                            tw.WriteLine(ex.InnerException.StackTrace);
                        }

                        tw.Flush();
                        //tw.Close();
                    }
                }
                #endregion


                return Content("");
            }
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
