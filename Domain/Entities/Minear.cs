using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Minear
    {
        [Key]
        public int id { get; set; }
        public string mining_earnings { get; set; }
        public string fpps_mining_earnings { get; set; }
        public DateTime date { get; set; }
    }
}
