using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstract
{
    public interface IMinearRepository
    {
        IEnumerable<Minear> Minears { get; set; }
        void UpdateEar(string Jsonstring);
    }
}
