using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Aksl.Net.Http
{
    public class WebApiService : IWebApiService
    {
        private const string applicationJson = "application/json";
        private static HttpClient _httpClient;
        //  private static readonly Lazy<HttpClient> _httpClient;

        static WebApiService()
        {
            //var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };Initializes
            _httpClient = new HttpClient();
            // var header = new MediaTypeWithQualityHeaderValue(applicationJson);
            //   _httpClient.DefaultRequestHeaders.Accept.Add(header);
            _httpClient.MaxResponseContentBufferSize = 1024 * 1024;//1M
            _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");

            //_httpClient = new Lazy<HttpClient>(() =>
            //{
            //    var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            //    var http = new HttpClient(handler);
            //    var header = new MediaTypeWithQualityHeaderValue(applicationJson);
            //    http.DefaultRequestHeaders.Accept.Add(header);
            //    http.MaxResponseContentBufferSize = 1024 * 1024;//1M
            //    return http;
            //});
        }

        public WebApiService() { }

        public HttpClient WebApiClient => _httpClient;

        //private HttpClient WebApiClient => _httpClient.Value;

        public string SerializeAsString<T>(T item) => JsonSerializer.Serialize(item);

        public T DeserializeAsString<T>(string json) => JsonSerializer.Deserialize<T>(json);

        public async Task<T> DeserializeAsStreamAsync<T>(HttpResponseMessage response) =>
              await OnDeserializeAsStreamAsync<T>(response);

        private async Task<T> OnDeserializeAsStreamAsync<T>(HttpResponseMessage response)
        {
            var stream = await response.Content.ReadAsStreamAsync();
            var djs = new DataContractJsonSerializer(typeof(T));
            var t = (T)djs.ReadObject(stream);
            stream.Dispose();

            return t;
        }

        private HttpContent OnSerializeAsStream<T>(T t)
        {
            var ms = new MemoryStream();
            var djs = new DataContractJsonSerializer(typeof(T));
            djs.WriteObject(ms, t);
            ms.Position = 0;

            var content = new StreamContent(ms);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return content;
        }

        private async Task<T> GetAsyncCore<T>(string requestUri)
        {
            //  _httpClient = new HttpClient();

            T t = default;

            //  _httpClient.MaxResponseContentBufferSize = 1024 * 1024;//1M
            var response = await WebApiClient.GetAsync(new Uri(requestUri));

            #region NET451
            //#if NET451
            var payload = await response.Content.ReadAsStringAsync();
            t = DeserializeAsString<T>(payload);
            //#else
            #endregion

            #region NETSTANDARD1_6
            //using (var stream = await response.Content.ReadAsStreamAsync())
            //{
            //    var djs = new DataContractJsonSerializer(typeof(T));
            //    t = (T)djs.ReadObject(stream);
            //}
            //#endif
            #endregion

            return t;
        }

        public async Task<T> GetAsync<T>(string requestUri)
        {
            return await GetAsyncCore<T>(requestUri);
        }

        public async Task<IEnumerable<T>> GetManyAsync<T>(string requestUri)
        {
            return await GetAsyncCore<IEnumerable<T>>(requestUri);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string requestUri, T t)
        {
            #region NET451
            //#if NET451
            var data = SerializeAsString(t);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await WebApiClient.PostAsync(new Uri(requestUri), content);
            return response;
            #endregion

            //return response;

            #region NETSTANDARD1_6
            //#endif
            //#if NETSTANDARD1_6
            //using (var ms = new MemoryStream())
            //{
            //    var djs = new DataContractJsonSerializer(typeof(T));
            //    djs.WriteObject(ms, t);
            //    ms.Position = 0;

            //    var content = new StreamContent(ms);
            //    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //    var response = await WebApiClient.PostAsync(new Uri(requestUri), content);
            //    return response;
            //}
            //#endif
            #endregion
        }

        public async Task<TResult> PostAsync<TInstance, TResult>(string requestUri, TInstance instance)
        {
            TResult newTResult = default(TResult);

            #region NET451
            //#if NET451
            var data = SerializeAsString(instance);//序列化成字符串
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await WebApiClient.PostAsync(new Uri(requestUri), content);

            var payload = await response.Content.ReadAsStringAsync();
            newTResult = DeserializeAsString<TResult>(payload);//反序列化结果

            #endregion

            #region NETSTANDARD1_6
            //#else
            //using (var ms = new MemoryStream())
            //{
            //    var djs = new DataContractJsonSerializer(typeof(TInstance));
            //    djs.WriteObject(ms, instance);
            //    ms.Position = 0;

            //    var content = new StreamContent(ms);
            //    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //    var response = await WebApiClient.PostAsync(new Uri(requestUri), content);
            //    using (var stream = await response.Content.ReadAsStreamAsync())
            //    {
            //        var dcjs = new DataContractJsonSerializer(typeof(TResult));
            //        newTResult = (TResult)dcjs.ReadObject(stream);
            //    }

            //    //  response.EnsureSuccessStatusCode();
            //}
            //#endif
            #endregion

            return newTResult;
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string requestUri, T t)
        {
            #region NET451
            //#if NET451
            var data = SerializeAsString(t);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await WebApiClient.PutAsync(new Uri(requestUri), content);
            return response;
            #endregion

            #region NETSTANDARD1_6
            //#else
            //using (var ms = new MemoryStream())
            //{
            //    var djs = new DataContractJsonSerializer(typeof(T));
            //    djs.WriteObject(ms, t);
            //    ms.Position = 0;

            //    var content = new StreamContent(ms);
            //    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //    var response = await WebApiClient.PutAsync(new Uri(requestUri), content);
            //    return response;
            //}
            //#endif
            #endregion
        }

        public async Task<TResult> PutAsync<TInstance, TResult>(string requestUri, TInstance instance)
        {
            TResult newTResult = default(TResult);

            #region NET451
            //#if NET451
            var data = SerializeAsString(instance);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await WebApiClient.PutAsync(new Uri(requestUri), content);

            var payload = await response.Content.ReadAsStringAsync();
            newTResult = DeserializeAsString<TResult>(payload);//反序列化结果
            #endregion

            #region NETSTANDARD1_6
            //#else
            //using (var ms = new MemoryStream())
            //{
            //    var djs = new DataContractJsonSerializer(typeof(TInstance));
            //    djs.WriteObject(ms, instance);
            //    ms.Position = 0;

            //    var content = new StreamContent(ms);
            //    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //    var response = await WebApiClient.PutAsync(requestUri, content);
            //    using (var stream = await response.Content.ReadAsStreamAsync())
            //    {
            //        var dcjs = new DataContractJsonSerializer(typeof(TResult));
            //        newTResult = (TResult)dcjs.ReadObject(stream);
            //    }
            //}
            //#endif
            #endregion

            return newTResult;
        }

        public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            var response = await WebApiClient.DeleteAsync(requestUri);
            //   response.EnsureSuccessStatusCode();
            return response;
        }

        //public void Dispose()
        //{
        //    //WebApiClient.Dispose();
        //}
    }
}
