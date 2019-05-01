using Newtonsoft.Json;
using RaceDay.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace RaceDay.Services
{
    public class RecaptchaClient
    {
        private const string RECAPTCHA_URL = "https://www.google.com/recaptcha/api/siteverify";

        public static async Task<VerifyResult> Verify(string secretKey, string clientResponse)
        {
            // Creeate the POST parameters
            //
            var postContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("secret", secretKey),
                new KeyValuePair<string, string>("response", clientResponse)
            });
           
            // Configure the POST client
            //
            using (HttpClient client = new HttpClient())
            {
                var response = await client.PostAsync(RECAPTCHA_URL, postContent);
                return JsonConvert.DeserializeObject<VerifyResult>(await response.Content.ReadAsStringAsync());
            }
        }
    }
    public class VerifyResult
    {
        public bool success;
        public string challenge_ts;
        public string hostnme;
    }

}