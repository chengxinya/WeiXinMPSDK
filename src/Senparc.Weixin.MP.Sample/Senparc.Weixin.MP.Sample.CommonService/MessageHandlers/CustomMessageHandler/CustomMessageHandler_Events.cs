/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc
    
    文件名：CustomMessageHandler_Events.cs
    文件功能描述：自定义MessageHandler
    
    
    创建标识：Senparc - 20170331
----------------------------------------------------------------*/

using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Sample.MySQL.Models;
using System;

namespace Senparc.Weixin.MP.Sample.CommonService.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// </summary>
    public partial class CustomMessageHandler
    {

        private string GetWelcomeInfo()
        {
            //获取Senparc.Weixin.MP.dll版本信息
            //var fileVersionInfo = FileVersionInfo.GetVersionInfo(Server.MapPath("~/bin/Senparc.Weixin.MP.dll"));
            //var version = string.Format("{0}.{1}.{2}", fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart, fileVersionInfo.FileBuildPart);
            return string.Format(
@"欢迎关注【Senparc.Weixin.MP 微信公众平台SDK】，当前运行版本：v{0}。
        您可以发送【文字】【位置】【图片】【语音】等不同类型的信息，查看不同格式的回复。

        您也可以直接点击菜单查看各种类型的回复。
        还可以点击菜单体验微信支付。

        SDK官方地址：https://weixin.senparc.com
        SDK Demo：http://sdk.weixin.senparc.com
        源代码及Demo下载地址：https://github.com/JeffreySu/WeiXinMPSDK
        Nuget地址：https://www.nuget.org/packages/Senparc.Weixin.MP
        QQ群：342319110

        ===============
        更多：

        1、JSSDK测试：http://sdk.weixin.senparc.com/WeixinJSSDK

        2、开放平台测试（建议PC上打开）：http://sdk.weixin.senparc.com/OpenOAuth/JumpToMpOAuth

        3、回复关键字：

        【open】   进入第三方开放平台（Senparc.Weixin.Open）测试

        【tm】     测试异步模板消息

        【openid】 获取OpenId等用户信息

        【约束】   测试微信浏览器约束

        【AsyncTest】 异步并发测试

        【错误】    体验发生错误无法返回正确信息

        【容错】    体验去重容错

        【ex】      体验错误日志推送提醒
        "
                /*,version*/);
        }

        public override IResponseMessageBase OnEvent_EnterRequest(RequestMessageEvent_Enter requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseMessage.Content = "您刚才发送了ENTER事件请求。";
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
        {
            //这里是微信客户端（通过微信服务器）自动发送过来的位置信息
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "这里写什么都无所谓，比如：上帝爱你！";
            return responseMessage;//这里也可以返回null（需要注意写日志时候null的问题）
        }

        //public override IResponseMessageBase OnEvent_ScanRequest(RequestMessageEvent_Scan requestMessage)
        //{
        //    //通过扫描关注
        //    var responseMessage = CreateResponseMessage<ResponseMessageText>();

        //    //下载文档
        //    if (!string.IsNullOrEmpty(requestMessage.EventKey))
        //    {
        //        var sceneId = long.Parse(requestMessage.EventKey.Replace("qrscene_", ""));
        //        //var configHelper = new ConfigHelper(new HttpContextWrapper(HttpContext.Current));
        //        var codeRecord =
        //            ConfigHelper.CodeCollection.Values.FirstOrDefault(z => z.QrCodeTicket != null && z.QrCodeId == sceneId);


        //        if (codeRecord != null)
        //        {
        //            //确认可以下载
        //            codeRecord.AllowDownload = true;
        //            responseMessage.Content = GetDownloadInfo(codeRecord);
        //        }
        //    }

        //    responseMessage.Content = responseMessage.Content ?? string.Format("通过扫描二维码进入，场景值：{0}", requestMessage.EventKey);



        //    return responseMessage;
        //}

        //public override IResponseMessageBase OnEvent_ViewRequest(RequestMessageEvent_View requestMessage)
        //{
        //    //说明：这条消息只作为接收，下面的responseMessage到达不了客户端，类似OnEvent_UnsubscribeRequest
        //    var responseMessage = CreateResponseMessage<ResponseMessageText>();
        //    responseMessage.Content = "您点击了view按钮，将打开网页：" + requestMessage.EventKey;
        //    return responseMessage;
        //}

        //public override IResponseMessageBase OnEvent_MassSendJobFinishRequest(RequestMessageEvent_MassSendJobFinish requestMessage)
        //{
        //    var responseMessage = CreateResponseMessage<ResponseMessageText>();
        //    responseMessage.Content = "接收到了群发完成的信息。";
        //    return responseMessage;
        //}

        /// <summary>
        /// 订阅（关注）事件
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);

            //获取用户信息
            var weixinUserInfo = UserApi.Info(appId, this.WeixinOpenId);

            if (weixinUserInfo != null)
            {

                var account = new Account()
                {
                    UserName = weixinUserInfo.nickname,
                    WeixinOpenId = weixinUserInfo.openid,
                    Email = "",
                    Password = "",
                    PasswordSalt = "",
                    AddTime = DateTime.Now,
                    Sex = (Senparc.Weixin.MP.Sample.MySQL.Models.Sex)weixinUserInfo.sex,
                    Country = weixinUserInfo.country,
                    City = weixinUserInfo.city,
                    HeadUrl = weixinUserInfo.headimgurl,
                    PicUrl = weixinUserInfo.headimgurl,
                };
                SenparcMysqlContext.Accounts.Add(account);
                SenparcMysqlContext.SaveChanges();
            }



            responseMessage.Content = GetWelcomeInfo();
            if (!string.IsNullOrEmpty(requestMessage.EventKey))
            {
                responseMessage.Content += "\r\n============\r\n场景值：" + requestMessage.EventKey;
            }
            return responseMessage;
        }

        /// <summary>
        /// 退订
        /// 实际上用户无法收到非订阅账号的消息，所以这里可以随便写。
        /// unsubscribe事件的意义在于及时删除网站应用中已经记录的OpenID绑定，消除冗余数据。并且关注用户流失的情况。
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_UnsubscribeRequest(RequestMessageEvent_Unsubscribe requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "有空再来";
            return responseMessage;
        }

        /// <summary>
        /// 事件之扫码推事件(scancode_push)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ScancodePushRequest(RequestMessageEvent_Scancode_Push requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之扫码推事件";
            return responseMessage;
        }

        /// <summary>
        /// 事件之扫码推事件且弹出“消息接收中”提示框(scancode_waitmsg)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ScancodeWaitmsgRequest(RequestMessageEvent_Scancode_Waitmsg requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之扫码推事件且弹出“消息接收中”提示框";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出拍照或者相册发图（pic_photo_or_album）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_PicPhotoOrAlbumRequest(RequestMessageEvent_Pic_Photo_Or_Album requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出拍照或者相册发图";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出系统拍照发图(pic_sysphoto)
        /// 实际测试时发现微信并没有推送RequestMessageEvent_Pic_Sysphoto消息，只能接收到用户在微信中发送的图片消息。
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_PicSysphotoRequest(RequestMessageEvent_Pic_Sysphoto requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出系统拍照发图";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出微信相册发图器(pic_weixin)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_PicWeixinRequest(RequestMessageEvent_Pic_Weixin requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出微信相册发图器";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出地理位置选择器（location_select）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_LocationSelectRequest(RequestMessageEvent_Location_Select requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出地理位置选择器";
            return responseMessage;
        }

        /// <summary>
        /// 事件之发送模板消息返回结果
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_TemplateSendJobFinishRequest(RequestMessageEvent_TemplateSendJobFinish requestMessage)
        {
            switch (requestMessage.Status)
            {
                case "success":
                    //发送成功
                    break;
                case "failed:user block":
                    //送达由于用户拒收（用户设置拒绝接收公众号消息）而失败
                    break;
                case "failed: system failed":
                    //送达由于其他原因失败
                    break;
                default:
                    throw new WeixinException("未知模板消息状态：" + requestMessage.Status);
            }

            //注意：此方法内不能再发送模板消息，否则会造成无限循环！

            //无需回复文字内容
            //return requestMessage
            //    .CreateResponseMessage<ResponseMessageNoResponse>();
            return null;
        }
    }
}