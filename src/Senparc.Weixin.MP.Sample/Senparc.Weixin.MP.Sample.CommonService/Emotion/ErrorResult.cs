/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：ErrorResult.cs
    文件功能描述：情绪识别错误信息


    创建标识：Senparc - 20170306

 ----------------------------------------------------------------*/
using System.Runtime.Serialization;

namespace Senparc.Weixin.MP.Sample.CommonService.Emotion
{
    /// <summary>
    /// 情绪识别返回错误信息
    /// </summary>
    [DataContract]
    public class ClientError
    {
        [DataMember(Name = "error")]
        public ClientExceptionMessage Error
        {
            get;
            set;
        }

    }

    [DataContract]
    public class ClientExceptionMessage
    {
        [DataMember(Name = "code")]
        public string ErrorCode
        {
            get;
            set;
        }

        [DataMember(Name = "message")]
        public string Message
        {
            get;
            set;
        }
    }


    /// <summary>
    /// 情绪识别返回错误信息
    /// </summary>
    [DataContract]
    public class ServiceError
    {
        [DataMember(Name = "statusCode")]
        public string ErrorCode
        {
            get;
            set;
        }

        [DataMember(Name = "message")]
        public string Message
        {
            get;
            set;
        }
    }
}
