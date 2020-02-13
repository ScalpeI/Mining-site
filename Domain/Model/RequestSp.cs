using Domain.Concrete;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;

namespace Domain.Model
{
    public class RequestSp
    {


        public async void Upload()
        {
            EFSpRepository repository = new EFSpRepository();
            EFUserRepository user = new EFUserRepository();
            foreach (var useronce in user.Users)
            {
                if (useronce.Stoken != null)
                {
                    try
                    {

                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        var req = WebRequest.Create(@"https://slushpool.com/accounts/profile/json/btc/");
                        req.Headers.Add("SlushPool-Auth-Token:" + useronce.Stoken);
                        var r = await req.GetResponseAsync();

                        StreamReader responseReader = new StreamReader(r.GetResponseStream());
                        var responseData = await responseReader.ReadToEndAsync();
                        JObject obj = JObject.Parse(responseData);
                            dynamic jsonDe = JsonConvert.DeserializeObject(obj.ToString());

                            //Console.WriteLine(jsonDe["username"].ToString()+" "+ float.Parse(jsonDe["btc"]["hash_rate_5m"].ToString(), CultureInfo.InvariantCulture) + " " + jsonDe["btc"]["hash_rate_unit"].ToString());
                            if (float.Parse(jsonDe["btc"]["hash_rate_5m"].ToString(), CultureInfo.InvariantCulture) > 0)
                                repository.Create(useronce.Login, float.Parse(jsonDe["btc"]["hash_rate_5m"].ToString(), CultureInfo.InvariantCulture), jsonDe["btc"]["hash_rate_unit"].ToString());
                            else repository.Create(useronce.Login, 0, "Th/s");

                        Thread.Sleep(5000);
                    }
                    catch 
                    {
                        repository.Create(useronce.Login, 0, "Th/s");
                    }
                }

            }
        }
    }
}
