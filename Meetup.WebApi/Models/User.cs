using FluentValidator;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Meetup.WebApi.Models
{
    public class User : Notifiable
    {
        static readonly char[] _caracteresNovaSenha = "abcdefghijklmnopqrstuvzwyz1234567890*-_".ToCharArray();
        static readonly int _newPasswordLenght = 10;

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

        public string Reset()
        {
            var senha = string.Empty;
            for (int i = 0; i < _newPasswordLenght; i++)
                senha += new Random().Next(0, _caracteresNovaSenha.Length);

            Password = Cipher(senha);

            return senha;
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

        public bool CheckPassword(string plainPassword)
        {
            return Cipher(plainPassword) == Password;
        }

    }
}
