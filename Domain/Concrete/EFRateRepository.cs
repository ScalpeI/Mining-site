using Domain.Abstract;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Domain.Concrete
{
    public class EFRateRepository : IRateRepository
    {
        EFDbContext context = new EFDbContext();
        public IEnumerable<Rate> Rates
        {
            get { return context.Rates; }
            set { }
        }
        public void SaveRate(Rate rate)
        {
            context.Rates.Attach(rate);
            context.Entry(rate).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}

