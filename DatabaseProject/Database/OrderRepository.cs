using Dapper;
using DatabaseProject.Consts;
using DatabaseProject.Models;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace DatabaseProject.Database
{
    class OrderRepository : IRepository<Order>
    {
        private static OrderRepository instance;

        private string connectionStr = "";

        public static OrderRepository GetInstance()
        {
            if (instance == null)
            {
                instance = new OrderRepository();
            }
            return instance;
        }

        private OrderRepository()
        {
            connectionStr = ConfigurationManager.ConnectionStrings[Constants.ConnectionStrName].ConnectionString;
        }

        public async Task<IEnumerable<Order>> GetListAsync()
        {
            using (SQLiteConnection connection = new(connectionStr))
            {
                connection.Open();
                string sql = "SELECT * FROM orders o LEFT JOIN users u ON o.userId = u.id LEFT JOIN products p ON o.productId = p.id";
                await Task.Delay(3000); //simulating long-running database response
                return await connection.QueryAsync<Order, User, Product, Order>(sql, (order, user, product) =>
                {
                    order.User = user;
                    order.Product = product;
                    return order;
                });
            }
        }

        public void Insert(Order order)
        {
            using (SQLiteConnection connection = new(connectionStr))
            {
                connection.Open();
                string sql = "INSERT INTO orders (id, date, userId, productId) VALUES (@Id, @Date, @UserId, @ProductId)";
                connection.Execute(sql, new { order.Id, order.Date, order.UserId, order.ProductId });
            }
        }

        public int Delete(string orderId)
        {
            using (SQLiteConnection connection = new(connectionStr))
            {
                connection.Open();
                string sql = "DELETE FROM orders WHERE id = @oId";
                return connection.Execute(sql, new { oId = orderId } );
            }
        }

        public Order Find(string orderId)
        {
            Order order;
            using (var connection = new SQLiteConnection(connectionStr))
            {
                connection.Open();
                string sql = "SELECT * FROM orders o LEFT JOIN users u ON o.userId = u.id LEFT JOIN products p ON o.productId = p.id WHERE o.id = @id";
                order = connection.Query<Order, User, Product, Order>(sql, (order, user, product) =>
                {
                    order.User = user;
                    order.Product = product;
                    return order;
                }, new { id = orderId }).FirstOrDefault();
            }
            return order;
        }

        public int FindByUsername(string name)
        {
            using (var connection = new SQLiteConnection(connectionStr))
            {
                connection.Open();
                string sql = "SELECT u.id FROM users u LEFT JOIN orders o ON o.userId = u.id WHERE u.username = @username";
                return connection.Query<int>(sql, new { username = name }).FirstOrDefault();
            }
        }

        public int Update(string oldName, string newName)
        {
            return -1;
        }

    }
}
