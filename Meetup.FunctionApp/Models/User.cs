using FluentValidator;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Meetup.FunctionApp.Models
{
    public class User: Notifiable
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }

        // Construtor padrão para o Entity Framework
        protected User()
        {
        }

        public User(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Id = Guid.NewGuid();

            if (!string.IsNullOrWhiteSpace(password))
                Password = Cipher(password);

            new ValidationContract<User>(this)
                .IsRequired(p => p.Name)
                .IsEmail(p => p.Email)
                .IsRequired(p => p.Password);

        }

        private string Cipher(string senha)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.Default.GetBytes(Email + "|aa28798a-dbcb-4c76-8c45-5a921745e10a|" + senha);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("x2"));

            return sb.ToString();
        }
    }
}
