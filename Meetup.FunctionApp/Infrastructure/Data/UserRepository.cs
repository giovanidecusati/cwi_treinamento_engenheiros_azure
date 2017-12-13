using Meetup.FunctionApp.Models;
using System;
using System.Data.SqlClient;

namespace Meetup.FunctionApp.Infrastructure.Data
{
    public class UserRepository : IDisposable
    {
        private SqlConnection _connection;

        public UserRepository()
        {
            _connection = new SqlConnection(FunctionHelper.GetEnvironmentVariable("SqlConnection"));
            _connection.Open();
        }

        public void Create(User user)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO [Users] (Id, Name, Email, Password) VALUES (@Id, @Name, @Email, @Password)";
                command.Parameters.AddWithValue("@Id", user.Id);
                command.Parameters.AddWithValue("@Name", user.Name);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.ExecuteNonQuery();
            }

        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
