using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Payout
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Логин")]
        public string owner { get; set; }
        [Display(Name = "Сумма BTC")]
        public double count { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Дата создания")]
        public DateTime date { get; set; }
        [Display(Name = "Подтверждение")]
        public bool status { get; set; }
    }
}
