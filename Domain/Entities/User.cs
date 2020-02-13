using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Domain.Entities
{
    public class User
    {
        [HiddenInput(DisplayValue =false)]
        [Display(Name ="ID")]
        public int UserId { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Логин")]
        public string Login { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [HiddenInput(DisplayValue = false)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string PasswordHash { get; set; }

        [Display(Name = "Ключ MMR")]
        public string Mkey { get; set; }

        [Display(Name = "Секрет MMR")]
        public string Msecret { get; set; }

        [Display(Name = "Ключ BTC")]
        public string Bkey { get; set; }

        [Display(Name = "BTC puid")]
        public string Bpuid { get; set; }

        [Display(Name = "Токен Spool")]
        public string Stoken { get; set; }

        [Display(Name = "Коэффициент")]
        public double Coef { get; set; }

    }
}
