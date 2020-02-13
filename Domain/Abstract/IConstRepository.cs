using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstract
{
    public interface IConstRepository
    {
        IEnumerable<Const> Consts { get; set; }
        void Update(Const @const);
    }
}
