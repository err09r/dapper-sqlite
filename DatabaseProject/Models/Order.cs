using DatabaseProject.Consts;

namespace DatabaseProject.Models
{
    public class Order : IModel
    {
        public string Id { get; }
        public string Date { get; }
        public int UserId { get; }
        public int ProductId { get; }

        private User _user;
        public User User
        {
            get
            {
                return _user;
            }

            set
            {
                if (_user == null)
                {
                    _user = value;
                }
            }
        }

        private Product _product;
        public Product Product
        {
            get
            {
                return _product;
            }

            set
            {
                if (_product == null)
                {
                    _product = value;
                }
            }
        }

        public Order(string id, string date, long userId, long productId)
        {
            Id = id;
            Date = date;
            UserId = (int)userId;
            ProductId = (int)productId;
        }

        public override string ToString()
        {
            return $"Order ID: {Id} || Amount: {_product.Price}{Constants.Currency} || Order date: {Date}\n" +
                $"\nUser: {_user.Username}, Email: {_user.Email}, Address: {_user.Address}" +
                $"\nProduct: {_product.Name}, Details: {_product.Descr}";
        }

    }
}
