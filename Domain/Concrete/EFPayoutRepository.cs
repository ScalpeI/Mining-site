using Domain.Abstract;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Concrete
{
    public class EFPayoutRepository : IPayoutRepository
    {
        EFDbContext context = new EFDbContext();
        public IEnumerable<Payout> Payouts
        {
            get { return context.Payouts; }
            set { }
        }
        public void Create(Payout payout)
        {
            context.Payouts.Add(payout);
            context.SaveChanges();
        }
        public void Edit(Payout payout)
        {
            context.Payouts.Attach(payout);
            context.Entry(payout).State = EntityState.Modified;
            context.SaveChanges();
        }
        public void Delete(Payout payout)
        {
            context.Entry(payout).State = EntityState.Deleted;
            context.Payouts.Remove(payout);
            context.SaveChanges();
        }
    }
    
}
