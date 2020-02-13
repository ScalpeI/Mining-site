using Domain.Concrete;
using Domain.Entities;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace Domain.Model
{
   public class RequestRate
    {
        public async void Upload()
        {
            EFRateRepository repository = new EFRateRepository();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var req = WebRequest.Create("https://nl.bitstamp.net/api/ticker/");
            var r = await req.GetResponseAsync();
            StreamReader responseReader = new StreamReader(r.GetResponseStream());
            var responseData = await responseReader.ReadToEndAsync();
            Rate rate = JsonConvert.DeserializeObject<Rate>(responseData);
            repository.SaveRate(rate);
        }
    }
}
