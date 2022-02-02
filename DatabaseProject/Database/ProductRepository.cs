using Dapper;
using DatabaseProject.Consts;
using DatabaseProject.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace DatabaseProject.Database
{
    class ProductRepository : IRepository<Product>
    {
        private static ProductRepository instance;

        private string connectionStr = "";

        public static ProductRepository GetInstance()
        {
            if (instance == null)
            {
                instance = new ProductRepository();
            }
            return instance;
        }

        private ProductRepository()
        {
            connectionStr = ConfigurationManager.ConnectionStrings[Constants.ConnectionStrName].ConnectionString;
        }

        public async Task<IEnumerable<Product>> GetListAsync()
        {
            using (SQLiteConnection connection = new(connectionStr))
            {
                connection.Open();
                await Task.Delay(3000); //simulating long-running database response
                return await connection.QueryAsync<Product>("SELECT id, name, price, descr FROM products");
            }
        }

        public void Insert(Product product)
        {
            using (SQLiteConnection connection = new(connectionStr))
            {
                connection.Open();
                string sql = "INSERT INTO products (name, price, descr) VALUES (@Name, @Price, @Descr)";
                connection.Execute(sql, new { product.Name, product.Price, product.Descr });
            }
        }

        public int Delete(string name)
        {
            using (SQLiteConnection connection = new(connectionStr))
            {
                connection.Open();
                string sql = "DELETE FROM products WHERE name = @pname";
                return connection.Execute(sql, new { pname = name });
            }
        }

        public int Update(string oldName, string newName)
        {
            using (var connection = new SQLiteConnection(connectionStr))
            {
                connection.Open();
                var sql = "UPDATE products SET name = @newName WHERE name = @oldName";
                return connection.Execute(sql, new { newName, oldName });
            }
        }

    }
}
