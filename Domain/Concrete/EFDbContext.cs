using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Concrete
{
    public class EFDbContext : DbContext
    {
        public DbSet<Rate> Rates { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Minear> Minears { get; set; }
        public DbSet<Mrr> Mrrs { get; set; }
        public DbSet<Btc> Btcs { get; set; }
        public DbSet<Sp> Sps { get; set; }
        public DbSet<Const> Consts { get; set; }
        public DbSet<Payout> Payouts { get; set; }
    }
}
