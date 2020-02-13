using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstract
{
    public interface IMrrRepository
    {
        IEnumerable<Mrr> Mrrs { get; set; }
        void CreateMrr(int idrig, string owner, double hash, string type);
        void DeleteMrr(Mrr mrr);
    }
}
