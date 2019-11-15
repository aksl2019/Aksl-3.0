using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aksl.Net.Http
{
    public interface IWebApiService
    {
        HttpClient WebApiClient { get; }

        string SerializeAsString<T>(T item);

        T DeserializeAsString<T>(string json);

        Task<T> GetAsync<T>(string requestUri);

        Task<IEnumerable<T>> GetManyAsync<T>(string requestUri);

        //Task<HttpResponseMessage> PostAsync<T>(string requestUri, T t);

        //Task<T> PostAsync<T>(string requestUri, T t);

        Task<HttpResponseMessage> PostAsync<T>(string requestUri, T payload);

        Task<TResult> PostAsync<TInstance, TResult>(string requestUri, TInstance instance);

        Task<HttpResponseMessage> PutAsync<T>(string requestUri, T t);

        Task<TResult> PutAsync<TInstance, TResult>(string requestUri, TInstance instance);

        //Task<HttpResponseMessage> PutAsync<T>(string requestUri, T t);

        Task<HttpResponseMessage> DeleteAsync(string requestUri);
    }
}
