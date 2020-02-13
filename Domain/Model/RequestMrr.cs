using Domain.Concrete;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Domain.Model
{

    public class RequestMrr
    {

        public async Task<dynamic> GetResponseRig(string Mkey, string Msecret)
        {
            hash_hmac hmac = new hash_hmac();
            string Key = Mkey;
            string Secret = Msecret;
            double mtime = Math.Round((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds * 10000);
            string endpoint = "/rig/mine";
            string sign_string = Key + mtime.ToString() + endpoint;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var req = WebRequest.Create(@"https://www.miningrigrentals.com/api/v2" + endpoint);
            req.Headers.Add("x-api-sign:" + hmac.sha1(sign_string, Secret));
            req.Headers.Add("x-api-key:" + Key);
            req.Headers.Add("x-api-nonce:" + mtime.ToString());
            var r = await req.GetResponseAsync();
            StreamReader responseReader = new StreamReader(r.GetResponseStream());
            return await responseReader.ReadToEndAsync();
        }
        public async void Upload()
        {
            RequestBtc btc = new RequestBtc();
            EFMrrRepository repository = new EFMrrRepository();
            EFUserRepository user = new EFUserRepository();
            foreach (var useronce in user.Users)
            {
                if (useronce.Mkey != null && useronce.Msecret != null)
                {
                    try
                    {
                        var responseData = "";
                        string check = "False";
                        while (check == "False")
                        {
                            responseData = await GetResponseRig(useronce.Mkey, useronce.Msecret);
                            check = JObject.Parse(responseData)["success"].ToString();
                        }
                        JObject obj = JObject.Parse(responseData);
                        dynamic jsonDe = JsonConvert.DeserializeObject(obj["data"].ToString());
                        string ID = "";
                        foreach (JObject typeStr in jsonDe)
                        {
                            ID += typeStr["id"].ToString() + ";";
                        }
                        //Console.WriteLine(ID);
                        var req1 = WebRequest.Create(@"https://www.miningrigrentals.com/api/v2/rig/" + ID);
                        var r1 = await req1.GetResponseAsync();
                        StreamReader responseReader1 = new StreamReader(r1.GetResponseStream());
                        var responseData1 = await responseReader1.ReadToEndAsync();
                        JObject obj1 = JObject.Parse(responseData1);
                        double sum = 0;
                        try
                        {
                            dynamic jsonDe1 = JsonConvert.DeserializeObject(obj1["data"].ToString());
                            
                            foreach (JObject typeStr in jsonDe1)
                            {
                                //Console.WriteLine(float.Parse(typeStr["hashrate"]["last_5min"]["hash"].ToString(), CultureInfo.InvariantCulture) + " " + typeStr["hashrate"]["last_5min"]["hash"].ToString());
                                if (float.Parse(typeStr["hashrate"]["last_5min"]["hash"].ToString(), CultureInfo.InvariantCulture) > 0)
                                {
                                    repository.CreateMrr(int.Parse(typeStr["id"].ToString()), useronce.Login, float.Parse(typeStr["hashrate"]["last_5min"]["hash"].ToString(), CultureInfo.InvariantCulture), typeStr["hashrate"]["last_5min"]["type"].ToString());
                                    sum += float.Parse(typeStr["hashrate"]["last_5min"]["hash"].ToString(), CultureInfo.InvariantCulture);
                                }
                            }
                            if (sum == 0) btc.Upload(useronce); else btc.CreateZero(useronce);
                        }
                        catch 
                        {
                            if (sum == 0) btc.Upload(useronce); else btc.CreateZero(useronce);
                        }
                    }
                    catch (WebException ex)
                    {
                        Console.WriteLine(ex);
                        repository.CreateMrr(0, useronce.Login, 0, "th");
                        btc.Upload(useronce);
                    }
                }


            }
        }

    }

}

