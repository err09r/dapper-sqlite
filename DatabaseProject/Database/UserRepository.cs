using Dapper;
using DatabaseProject.Consts;
using DatabaseProject.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace DatabaseProject.Database
{
    class UserRepository : IRepository<User>
    {
        private static UserRepository instance;

        private string connectionStr = "";

        public static UserRepository GetInstance()
        {
            if (instance == null)
            {
                instance = new UserRepository();
            }
            return instance;
        }

        private UserRepository()
        {
            connectionStr = ConfigurationManager.ConnectionStrings[Constants.ConnectionStrName].ConnectionString;
        }

        public void Insert(User user)
        {
            using (SQLiteConnection connection = new(connectionStr))
            {
                connection.Open();
                string sql = "INSERT INTO users (username, regDate, email, address) VALUES (@Username, @RegDate, @Email, @Address)";
                connection.Execute(sql, new { user.Username, user.RegDate, user.Email, user.Address });
            }
        }

        public async Task<IEnumerable<User>> GetListAsync()
        {
            using (SQLiteConnection connection = new(connectionStr))
            {
                connection.Open();
                await Task.Delay(3000); //simulating long-running database response
                return await connection.QueryAsync<User>("SELECT id, username, regDate, email, address FROM users");
            }
        }

        public int Delete(string name)
        {
            using (SQLiteConnection connection = new(connectionStr))
            {
                connection.Open();
                string sql = "DELETE FROM users WHERE username = @username";
                return connection.Execute(sql, new { username = name });
            }
        }

        public int Update(string oldName, string newName)
        {
            using (var connection = new SQLiteConnection(connectionStr))
            {
                connection.Open();
                var sql = "UPDATE users SET username = @newName WHERE username = @oldName";
                return connection.Execute(sql, new { newName, oldName });
            }
        }
    }
}
