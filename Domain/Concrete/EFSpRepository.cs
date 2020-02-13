using Domain.Abstract;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Concrete
{
    public class EFSpRepository : ISpRepository
    {
        EFDbContext context = new EFDbContext();
        public IEnumerable<Sp> Sps
        {
            get { return context.Sps; }
            set { }
        }
        public void Create(string owner, double hash, string type)
        {
                context.Sps.Add(new Sp { owner = owner, hash = hash, type = type, date = DateTime.Parse(DateTime.Now.ToString("s").Substring(0, 17) + "00") });
                context.SaveChanges();
        }
    }
}
