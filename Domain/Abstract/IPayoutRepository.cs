using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstract
{
    public interface IPayoutRepository
    {
        IEnumerable<Payout> Payouts { get; set; }
        void Create(Payout payout);
        void Edit(Payout payout);
        void Delete(Payout payout);
    }
}
