using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [DataContract]
    public class Mrr
    {
        [Key]
        public int id { get; set; }

        public int idrig { get; set; }
        [DataMember]
        public string owner { get; set; }

        public double hash { get; set; }
        public string type { get; set; }
        [DataMember]
        public DateTime date { get; set; }
    }
}
