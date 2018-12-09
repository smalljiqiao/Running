using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HB.Common.Core.Framework.Helpers
{
  public class WeatherHelper
    {
        private const String host = "https://ali-weather.showapi.com";
        private const String path = "/spot-to-weather";
        private const String method = "GET";
        private const String AppKey = "24697091";
        private const String AppSecret = "58c9338752a96b7a5fd4795d1eab611f";
        private const String AppCode="198d91fcfac1401e8511f4f9d7d581c9";
        public string GetWeather(string cityName,out bool result) {
            if (string.IsNullOrEmpty(cityName)) {
                result = false;
                return ""; }
            string encodeCityName= System.Web.HttpUtility.UrlEncode(cityName);
            String querys = "area="+ encodeCityName + "&need3HourForcast=0&needAlarm=0&needHourData=0&needIndex=0&needMoreDay=0";
            String bodys = "";
            String url = host + path;
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;

            if (0 < querys.Length)
            {
                url = url + "?" + querys;
            }

            if (host.Contains("https://"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                httpRequest = (HttpWebRequest)WebRequest.Create(url);
            }
            httpRequest.Method = method;
            httpRequest.Headers.Add("Authorization", "APPCODE " + AppCode);
            if (0 < bodys.Length)
            {
                byte[] data = Encoding.UTF8.GetBytes(bodys);
                using (Stream stream = httpRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            try
            {
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            }
            catch (WebException ex)
            {
                httpResponse = (HttpWebResponse)ex.Response;
            }
            var back =httpResponse.StatusCode;
            Stream st = httpResponse.GetResponseStream();
            StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
            result = true;
            return  reader.ReadToEnd();
        }
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

    }
}
