using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstract
{
    public interface ISpRepository
    {
            IEnumerable<Sp> Sps { get; set; }
        void Create(string owner, double hash, string type);
    }
}
