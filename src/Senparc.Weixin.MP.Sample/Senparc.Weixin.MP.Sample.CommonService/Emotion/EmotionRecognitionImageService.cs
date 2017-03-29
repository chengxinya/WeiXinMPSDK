/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：EmotionRecognitionImageService.cs
    文件功能描述：情绪识别-Image服务。


    创建标识：Senparc - 20170306

 ----------------------------------------------------------------*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.Weixin.MP.Sample.CommonService.Emotion
{

    #region 返回值

    /// <summary>
    /// 情书识别 返回信息
    /// </summary>
    [DataContract]
    public class EmotionResult
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "faceRectangle")]
        public FaceRectangle faceRectangle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "scores")]
        public Scores Scores { get; set; }
    }


    [DataContract]
    public class FaceRectangle
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "height")]
        public int Height { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "left")]
        public int Left { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "top")]
        public int Top { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "width")]
        public int Width { get; set; }
    }


    [DataContract]
    public class Scores
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "anger")]
        public decimal Anger { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "contempt")]
        public decimal Contempt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "disgust")]
        public decimal Disgust { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "fear")]
        public decimal Fear { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "happiness")]
        public decimal Happiness { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "neutral")]
        public decimal Neutral { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "sadness")]
        public decimal Sadness { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "surprise")]
        public decimal Surprise { get; set; }
    }

    #endregion

    /// <summary>
    /// 情绪识别-Image
    /// </summary>
    class EmotionRecognitionImageService : IDisposable
    {
        #region 字段
        private const string SubscriptionKeyName = "ocp-apim-subscription-key";

        private const string ServiceHost = "https://westus.api.cognitive.microsoft.com/emotion/v1.0/recognize";

        private const string JsonContentTypeHeader = "application/json";

        private const string StreamContentTypeHeader = "application/octet-stream";

        private HttpClient _httpClient;

        #endregion

        public EmotionRecognitionImageService(string subscriptionKey)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add(SubscriptionKeyName, subscriptionKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="httpMethod"></param>
        /// <param name="requestUrl"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        private async Task<TResponse> SendRequestAsync<TRequest, TResponse>(HttpMethod httpMethod, string requestUrl, TRequest requestBody)
        {
            var request = new HttpRequestMessage(httpMethod, ServiceHost)
            {
                RequestUri = new Uri(requestUrl)
            };

            if (requestBody != null)
            {
                if (requestBody is Stream)
                {
                    request.Content = new StreamContent(requestBody as Stream);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(StreamContentTypeHeader);
                }
                else
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, JsonContentTypeHeader);
                }
            }

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = null;
                if (response.Content != null)
                {
                    responseContent = await response.Content.ReadAsStringAsync();
                }

                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    return JsonConvert.DeserializeObject<TResponse>(responseContent);
                }
            }
            else
            {
                if (response.Content != null && response.Content.Headers.ContentType.MediaType.Contains(JsonContentTypeHeader))
                {
                    var errorObjectString = await response.Content.ReadAsStringAsync();
                    ClientError ex = JsonConvert.DeserializeObject<ClientError>(errorObjectString);
                    if (ex.Error != null)
                    {
                        throw new Exception(string.Format("ErrorCode：{0} Message：{1} StatusCode：{2}", ex.Error.ErrorCode, ex.Error.Message, response.StatusCode));
                    }
                    else
                    {
                        ServiceError serviceEx = JsonConvert.DeserializeObject<ServiceError>(errorObjectString);
                        if (serviceEx != null)
                        {
                            throw new Exception(string.Format("ErrorCode：{0} Message：{1} StatusCode：{2}", serviceEx.ErrorCode, serviceEx.Message, response.StatusCode));
                        }
                        else
                        {
                            throw new Exception("Unknown Error StatusCode:" + response.StatusCode);
                        }
                    }
                }
                response.EnsureSuccessStatusCode();
            }
            return default(TResponse);
        }


        #region 同步方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        public List<EmotionResult> DetectEmotion(string imageUrl)
        {
            var result = SendRequestAsync<object, List<EmotionResult>>(HttpMethod.Post, ServiceHost, new
            {
                url = imageUrl
            });
            return result.Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageStream"></param>
        /// <returns></returns>
        public List<EmotionResult> DetectEmotion(Stream imageStream)
        {
            var result = SendRequestAsync<Stream, List<EmotionResult>>(HttpMethod.Post, ServiceHost, imageStream);
            return result.Result;
        }

        #endregion

        #region 异步方法
        public async Task<List<EmotionResult>> DetectEmotionAsync(string imageUrl)
        {
            var result = await SendRequestAsync<object, List<EmotionResult>>(HttpMethod.Post, ServiceHost, new
            {
                url = imageUrl
            });
            return result;
        }

        public async Task<List<EmotionResult>> DetectEmotionAsync(Stream imageStream)
        {
            var result = await SendRequestAsync<Stream, List<EmotionResult>>(HttpMethod.Post, ServiceHost, imageStream);
            return result;
        }
        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_httpClient != null)
                {
                    _httpClient.Dispose();
                    _httpClient = null;
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
