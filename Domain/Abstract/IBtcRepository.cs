using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstract
{
    public interface IBtcRepository
    {
            IEnumerable<Btc> Btcs { get; set; }
        /// <summary>
        /// Создает запись в таблице Btcs
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="hash"></param>
        /// <param name="type"></param>
        void Create(string owner, double hash, string type);
    }
}
