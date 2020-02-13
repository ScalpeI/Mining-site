using Domain.Abstract;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Concrete
{
    public class EFBtcRepository : IBtcRepository
    {
        EFDbContext context = new EFDbContext();
        public IEnumerable<Btc> Btcs
        {
            get { return context.Btcs; }
            set { }
        }
        /// <summary>
        /// Создает запись в таблице Btcs
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="hash"></param>
        /// <param name="type"></param>
        public void Create(string owner, double hash, string type)
        {
                context.Btcs.Add(new Btc { owner = owner, hash = hash, type = type, date = DateTime.Parse(DateTime.Now.ToString("s").Substring(0, 17) + "00") });
                context.SaveChanges();
        }
    }
}
