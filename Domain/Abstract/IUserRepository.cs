using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstract
{
    public interface IUserRepository
    {
        IEnumerable<User> Users { get; set; }
        
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
    }
}
