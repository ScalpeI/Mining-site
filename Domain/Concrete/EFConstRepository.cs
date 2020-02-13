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
    public class EFConstRepository : IConstRepository
    {
        EFDbContext context = new EFDbContext();
        public IEnumerable<Const> Consts
        {
            get { return context.Consts; }
            set { }
        }
        public void Update(Const @const)
        {

            context.Consts.Attach(@const);
            context.Entry(@const).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}
