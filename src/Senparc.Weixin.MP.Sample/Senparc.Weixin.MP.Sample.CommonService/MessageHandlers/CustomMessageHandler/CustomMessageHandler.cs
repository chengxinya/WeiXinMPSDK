/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：CustomMessageHandler.cs
    文件功能描述：微信公众号自定义MessageHandler


    创建标识：Senparc - 20170331
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.MP.Sample.CommonService.Emotion;
using Senparc.Weixin.MP.Sample.MySQL.Models;
using System.Threading.Tasks;
using Senparc.Weixin.MP.AdvancedAPIs;

namespace Senparc.Weixin.MP.Sample.CommonService.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// 把MessageHandler作为基类，重写对应请求的处理方法
    /// </summary>
    public partial class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {
        private readonly static string appId = "wxe273c3a02e09ff8c";
        //private readonly static string appSecret = "631f30445f640e1a870f1ef79aa543bd";
        private readonly static string subscriptionKey = "9368e4cd242c4761a7b5fe8e7087c0ca";

        SenparcContext senparcMysqlContext = null;

        private SenparcContext SenparcMysqlContext
        {
            get
            {
                if (senparcMysqlContext == null)
                {
                    senparcMysqlContext = new SenparcContext("Server=senparcsdk.mysqldb.chinacloudapi.cn;Port=3306;Database=test;Uid=senparcsdk%mysql;Pwd=!@#EWQASD123;Connection Reset=false");
                }
                return senparcMysqlContext;
            }
        }

        /// <summary>
        /// 模板消息集合（Key：checkCode，Value：OpenId）
        /// </summary>
        public static Dictionary<string, string> TemplateMessageCollection = new Dictionary<string, string>();

        public CustomMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            //在指定条件下，不使用消息去重
            base.OmitRepeatedMessageFunc = requestMessage =>
            {
                var textRequestMessage = requestMessage as RequestMessageText;
                if (textRequestMessage != null && textRequestMessage.Content == "容错")
                {
                    return false;
                }
                return true;
            };
        }

        public CustomMessageHandler(RequestMessageBase requestMessage)
            : base(requestMessage)
        {
        }

        public override void OnExecuting()
        {
            //测试MessageContext.StorageData
            if (CurrentMessageContext.StorageData == null)
            {
                CurrentMessageContext.StorageData = 0;
            }
            base.OnExecuting();
        }

        public override void OnExecuted()
        {
            base.OnExecuted();
            CurrentMessageContext.StorageData = ((int)CurrentMessageContext.StorageData) + 1;
        }

        /// <summary>
        /// 处理图片请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        {
            //推送消息
            Task.Factory.StartNew(async () =>
            {
                await CustomApi.SendTextAsync(appId, base.WeixinOpenId, "已收到您发送的图片，正在处理...");

                await CustomApi.SendTextAsync(appId, base.WeixinOpenId, requestMessage.PicUrl);


                await CustomApi.SendImageAsync(appId, base.WeixinOpenId, requestMessage.MediaId);
            });

            //TODO:调用表情识别接口
            List<EmotionResult> result = null;
            string description = null;
            try
            {
                EmotionRecognitionImageService service = new EmotionRecognitionImageService(subscriptionKey);

                //var ms = new MemoryStream();
                //Senparc.Weixin.HttpUtility.Get.Download(requestMessage.PicUrl, ms);
                //ms.Seek(0, SeekOrigin.Begin);
                //result = service.DetectEmotion(ms);

                result = service.DetectEmotion(requestMessage.PicUrl);

                //TODO：保存数据
                if (result != null)
                {
                    var account = SenparcMysqlContext.Accounts.FirstOrDefault(z => z.WeixinOpenId == WeixinOpenId);
                    if (account != null)
                    {
                        var cognitiveEmotion = new CognitiveEmotion()
                        {
                            AccountId = account.Id,
                            ResultJson = Newtonsoft.Json.JsonConvert.SerializeObject(result),
                            PicUrl = requestMessage.PicUrl,
                            AddTime = DateTime.Now
                        };
                        SenparcMysqlContext.CognitiveEmotions.Add(cognitiveEmotion);
                        SenparcMysqlContext.SaveChanges();

                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                        responseMessage.Articles.Add(new Article()
                        {
                            Title = "点击查看表情识别信息",
                            Description = description,
                            PicUrl = requestMessage.PicUrl,
                            Url = $"http://senparcsdkmysql.chinacloudsites.cn/Home/EmotionLog?accountId={account.Id}"
                        });
                        return responseMessage;
                    }
                }
                //description = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            }
            catch (System.Exception ex)
            {
                result = new List<EmotionResult>();
                description = ex.Message;
            }
            finally
            {
                description = description ?? (result.Count > 0 ? Newtonsoft.Json.JsonConvert.SerializeObject(result) : "表情识别失败！");
            }


            //var responseMessage = CreateResponseMessage<ResponseMessageNews>();
            //responseMessage.Articles.Add(new Article()
            //{
            //    Title = "您刚才发送了图片信息",
            //    Description = "您发送的图片将会显示在边上",
            //    PicUrl = requestMessage.PicUrl,
            //    Url = "http://sdk.weixin.senparc.com"
            //});

            var responseTestMessage = CreateResponseMessage<ResponseMessageText>();
            responseTestMessage.Content = "您的表情结果：" + description;
            return responseTestMessage;
        }

        /// <summary>
        /// 处理链接消息请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnLinkRequest(RequestMessageLink requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseMessage.Content = string.Format(@"您发送了一条连接信息：
Title：{0}
Description:{1}
Url:{2}", requestMessage.Title, requestMessage.Description, requestMessage.Url);
            return responseMessage;
        }

        /// <summary>
        /// 处理事件请求（这个方法一般不用重写，这里仅作为示例出现。除非需要在判断具体Event类型以外对Event信息进行统一操作
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEventRequest(IRequestMessageEventBase requestMessage)
        {
            var eventResponseMessage = base.OnEventRequest(requestMessage);//对于Event下属分类的重写方法，见：CustomerMessageHandler_Events.cs
                                                                           //TODO: 对Event信息进行统一操作
            return eventResponseMessage;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            /* 所有没有被处理的消息会默认返回这里的结果，
            * 因此，如果想把整个微信请求委托出去（例如需要使用分布式或从其他服务器获取请求），
            * 只需要在这里统一发出委托请求，如：
            * var responseMessage = MessageAgent.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
            * return responseMessage;
            */

            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "这条消息来自DefaultResponseMessage。";
            return responseMessage;
        }
    }
}
