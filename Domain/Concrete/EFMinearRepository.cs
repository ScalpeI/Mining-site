using Domain.Abstract;
using Domain.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Concrete
{
    public class EFMinearRepository : IMinearRepository
    {
        EFDbContext context = new EFDbContext();
        public IEnumerable<Minear> Minears
        {
            get { return context.Minears; }
            set { }
        }
        public void UpdateEar(string Jsonstring)
        {
            try
            {
                    JObject obj = JObject.Parse(Jsonstring);
                    dynamic jsonDe = JsonConvert.DeserializeObject(obj["data"].ToString());
                context.Minears.Add(new Minear { mining_earnings = jsonDe["mining_earnings"].ToString(), fpps_mining_earnings = "0.0000"+jsonDe["amount_standard_earn"].ToString() });
                    context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString() + " " + ex);
            }
        }
    }
}
