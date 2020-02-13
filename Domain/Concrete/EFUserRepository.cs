using Domain.Abstract;
using Domain.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Cryptography;

namespace Domain.Concrete
{
    public class EFUserRepository : IUserRepository
    {
        EFDbContext context = new EFDbContext();
        public IEnumerable<User> Users
        {
            get { return context.Users; }
            set { }
        }
        public void UpdateUser(User user)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            user.PasswordHash = Convert.ToBase64String(salt);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: user.Password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
            user.Password = hashed;
            context.Users.Attach(user);
                context.Entry(user).State = EntityState.Modified;
                context.SaveChanges();
        }
        public void CreateUser(User user)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            user.PasswordHash = Convert.ToBase64String(salt);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: user.Password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
            user.Password = hashed;
            context.Users.Add(user);
            context.SaveChanges();
        }
        public void DeleteUser(User user)
        {
            context.Entry(user).State = EntityState.Deleted;
            context.Users.Remove(user);
            context.SaveChanges();
        }
    }
}

