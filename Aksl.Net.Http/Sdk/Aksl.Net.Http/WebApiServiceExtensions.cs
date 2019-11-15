using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Aksl.Net.Http
{
    public static class WebApiServiceExtensions
    {
        public static Aksl.Net.Http.IWebApiService AddJsonHeader(this Aksl.Net.Http.IWebApiService webApiService)
        {
            var header = new MediaTypeWithQualityHeaderValue("application/json");
            webApiService.WebApiClient.DefaultRequestHeaders.Accept.Add(header);

            return webApiService;
        }

        public static Aksl.Net.Http.IWebApiService AddAuthHeaderValue(this Aksl.Net.Http.IWebApiService webApiService, string auToken)
        {
            webApiService.WebApiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auToken);

            return webApiService;
        }

        public static async Task<IEnumerable<TResult>> PostBatchAsync<TInstance, TResult>(this Aksl.Net.Http.IWebApiService webApiService, string requestUri, IEnumerable<TInstance> datas)
        {
            var results = new List<TResult>();

            foreach (var data in datas)
            {
                if (data != null)
                {
                    var result = await webApiService.PostAsync<TInstance, TResult>(requestUri, data);
                    if (result != null)
                    {
                        results.Add(result);
                    }
                }
            }

            return results;
        }

        public static async Task<IEnumerable<TResult>> BulkInsertAsync<TInstance, TResult>(this Aksl.Net.Http.IWebApiService webApiService, string requestUri, IEnumerable<TInstance> datas)
        {
            if (!((datas?.Any()).HasValue))
            {
                return default;
            }

            var results = await webApiService.PostAsync<IEnumerable<TInstance>, IEnumerable<TResult>>(requestUri, datas);
            return results;
        }

        //public static async Task<IEnumerable<TResult>> RecieveBatchAsync<TResult>(this Aksl.Net.Http.IWebApiService webApiService, string requestUri)
        //{
        //    var results = default(IEnumerable<TResult>);

        //    results = await webApiService.GetManyAsync<TResult>(requestUri);

        //    return results;
        //}
    }
}
