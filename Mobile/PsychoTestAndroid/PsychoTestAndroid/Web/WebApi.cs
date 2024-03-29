﻿using Android.App;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using Xamarin.Android.Net;
using System.Threading;
using PsychoTestAndroid.DataBase.Entity;
using PsychoTestAndroid.Helpers;

namespace PsychoTestAndroid.Web
{
    // Класс для работы с апи.
    public static class WebApi
    {
        // Ссылка на апи сервер.
        static string url; 
        private static AndroidClientHandler _socketsHttpHandler;
        private static AndroidClientHandler SocketsHttpHandler
        {
            get
            {
                if (_socketsHttpHandler == null)
                {
                    _socketsHttpHandler = new AndroidClientHandler()
                    {
                        ReadTimeout = TimeSpan.FromSeconds(5)
                    };
                    _socketsHttpHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                }
                return _socketsHttpHandler;
            }
        }
        public static string Token { get; private set; }
        static WebApi()
        {
            var context = Application.Context;

            url = PreferencesHelper.GetString("url", null);
            if (url == null)
            {
                url = context.GetString(Resource.String.base_url);
                url = "https://10.0.2.2:5001";
            }
            Token = PreferencesHelper.GetString("token", null);
        }

        public static async Task<HttpStatusCode?> Login(string url)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            var client = new HttpClient(clientHandler);
            //var client = new HttpClient(SocketsHttpHandler);
            HttpResponseMessage result;
            try
            {
                result = await client.GetAsync(url.Replace("ptest://", ""));
            }
            catch
            {
                return null;
            }
            if (result != null && result.StatusCode == HttpStatusCode.OK)
            {
                JObject data = JObject.Parse(await result.Content.ReadAsStringAsync());
                var newUrl = data["domainName"].ToString();
                var newToken = data["token"].ToString();
                if (newUrl == null || newToken == null)
                {
                    return null;
                }
                Token = newToken;
                url = newUrl;
                PreferencesHelper.PutString("url", url);
                PreferencesHelper.PutString("token", Token);
            }
            return result.StatusCode;
        }

        public static async Task<string> GetTest(string id)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            var client = new HttpClient(clientHandler);
            //var client = new HttpClient(SocketsHttpHandler);
            HttpResponseMessage result;
            try
            {
                result = await client.GetAsync(url + "api/tests/" + id);
            }
            catch
            {
                return null;
            }
            if (result != null && result.StatusCode == HttpStatusCode.OK)
            {
                return await result.Content.ReadAsStringAsync();
            }
            return null;
        }

        public static async Task<string> GetScale(string id)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            var client = new HttpClient(clientHandler);
            //var client = new HttpClient(SocketsHttpHandler);
            HttpResponseMessage result;
            try
            {
                result = await client.GetAsync(url + "api/Tests/norm/" + id);
            }
            catch
            {
                return null;
            }
            if (result != null && result.StatusCode == HttpStatusCode.OK)
            {
                return await result.Content.ReadAsStringAsync();
            }
            return null;
        }

        // Получить список доступных тестов.
        public static async Task<List<DbTest>> GetTests()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            var client = new HttpClient(clientHandler);
            //var client = new HttpClient(SocketsHttpHandler);
            List<DbTest> tests;
            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue(Token);
            request.RequestUri = new Uri(url + "api/tests");
            request.Method = HttpMethod.Get;
            HttpResponseMessage result;
            try
            {
                result = await client.SendAsync(request, CancellationToken.None);
            }
            catch
            {
                return new List<DbTest>();
            }
            if (result != null)
            {
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    tests = JsonConvert.DeserializeObject<List<DbTest>>(await result.Content.ReadAsStringAsync());
                    return tests;
                }
                if (result.StatusCode == HttpStatusCode.Forbidden)
                {
                    return null;
                }
            }
            return new List<DbTest>();
        }

        public static async Task<bool> SendResult(string testResult)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            var client = new HttpClient(clientHandler);
            //var client = new HttpClient(SocketsHttpHandler);
            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Authorization = new AuthenticationHeaderValue(Token);
            request.RequestUri = new Uri(url + "api/answers/");
            request.Method = HttpMethod.Post;
            request.Content = new StringContent(testResult, Encoding.Default, "application/json");
            HttpResponseMessage result;
            try
            {
                result = await client.SendAsync(request, CancellationToken.None);
            }
            catch
            {
                return false;
            }
            return result != null && result.StatusCode == HttpStatusCode.OK;
        }

        public static void RemoveToken()
        {
            Token = "";
            PreferencesHelper.PutString("token", "");
        }
    }
}