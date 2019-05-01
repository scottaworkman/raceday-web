using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RaceDay.Services
{
    public class RestClient
    {
        private Uri baseUrl;
        private List<KeyValuePair<string, string>> headers;

        public HttpStatusCode StatusCode { get; set; }

        public RestClient(string url)
        {
            baseUrl = new Uri(url);
            headers = new List<KeyValuePair<string, string>>();
        }

        public void AddHeader(string name, string value)
        {
            headers.Add(new KeyValuePair<string, string>(name, value));
        }

        public void ClearHeaders()
        {
            headers.Clear();
        }

        public async Task<T> PostApi<T>(string api, object value, HttpStatusCode success) where T : class
        {
            using (HttpClient rest = new HttpClient())
            {
                foreach(var header in headers)
                {
                    rest.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                try
                {
                    var response = await rest.PostAsJsonAsync(new Uri(baseUrl, api).ToString(), value);
                    StatusCode = response.StatusCode;
                    if (StatusCode == success)
                    {
                        return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                    }
                }
                catch (HttpRequestException)
                {
                }
            }

            return null;
        }

        public async Task<T> GetApi<T>(string api) where T : class
        {
            using (HttpClient rest = new HttpClient())
            {
                foreach (var header in headers)
                {
                    rest.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                try
                {
                    var response = await rest.GetAsync(new Uri(baseUrl, api).ToString());
                    StatusCode = response.StatusCode;

                    if ((StatusCode != HttpStatusCode.Unauthorized) && (StatusCode != HttpStatusCode.Forbidden))
                    {
                        return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                    }
                }
                catch(HttpRequestException)
                { 
                }
            }

            return null;
        }

        /// <summary>
        /// Simple PUT REST request with no request/response body
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        /// 
        public async Task PutApi(string api)
        {
            await SimpleApi(api, "PUT", null);
            return;
        }

        /// <summary>
        /// Simple PUT DELETE request with no request/response body
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        /// 
        public async Task DeleteApi(string api)
        {
            await SimpleApi(api, "DELETE", null);
            return;
        }

        /// <summary>
        /// Common method for making a REST call with an option request body and no response value
        /// </summary>
        /// <param name="api"></param>
        /// <param name="method"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// 
        public async Task SimpleApi(string api, string method, object value)
        {
            using (HttpClient rest = new HttpClient())
            {
                foreach (var header in headers)
                {
                    rest.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                if (method.ToUpper() == "PUT")
                {
                    var response = await rest.PutAsJsonAsync(new Uri(baseUrl, api).ToString(), value);
                    StatusCode = response.StatusCode;
                }
                else if (method.ToUpper() == "DELETE")
                {
                    var response = await rest.DeleteAsync(new Uri(baseUrl, api).ToString());
                    StatusCode = response.StatusCode;
                }
            }

            return;
        }
    }
}
