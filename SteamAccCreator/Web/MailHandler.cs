﻿using Newtonsoft.Json;
using RestSharp;
using System;
using System.Text.RegularExpressions;

namespace SteamAccCreator.Web
{
    public class MailHandler
    {
        private readonly RestClient _client = new RestClient();
        private readonly RestRequest _request = new RestRequest();

        public static Uri MailboxUri = new Uri(Program.DEFAULT_URL_MAILBOX);
        public static bool IsMailBoxCustom = false;
        private static readonly Uri SteamUri = new Uri("https://store.steampowered.com/account/newaccountverification?");

        public static int CheckUserMailVerifyCount = 300;
        public static int CheckRandomMailVerifyCount = 5;

        public void ConfirmMail(string address)
        {
            Logger.Trace($"Confirming mail: {address}");

            System.Threading.Thread.Sleep(5000);
            _client.BaseUrl = MailboxUri;

            _request.Method = Method.GET;
            _request.AddParameter("e", address);

            var response = _client.Execute(_request);
            dynamic jsonResponse;
            try
            {
                jsonResponse = JsonConvert.DeserializeObject(response.Content);
            }
            catch (JsonException jEx)
            {
                Logger.Error("Confirming mail: JSON deserialize error.", jEx);

                jsonResponse = "";
                return;
            }
            catch (Exception ex)
            {
                Logger.Error("Confirming mail: Error.", ex);

                jsonResponse = "";
                return;
            }
            _request.Parameters.Clear();
            try
            {
                string dataxx = jsonResponse.First.ToString();
                string[] words = (Regex.Split(dataxx, "stoken="));
                string[] words9 = (Regex.Split(words[1], "&"));
                string[] words1 = (Regex.Split(dataxx, "creationid="));
                string[] words2 = Regex.Split(words1[1], " ");
                var tokenUri = "stoken=" + words9[0] + "&creationid=" + words2[0];
                ConfirmSteamAccount(new Uri(SteamUri + tokenUri));

                Logger.Trace("Confirming mail: done?");
            }
            catch (Exception ex)
            {
                Logger.Error($"Confirming mail: Error", ex);
            }
        }

        private void ConfirmSteamAccount(Uri uri)
        {
            _client.BaseUrl = uri;
            _request.Method = Method.GET;
            _client.Execute(_request);
            _request.Parameters.Clear();
        }
    }
}
