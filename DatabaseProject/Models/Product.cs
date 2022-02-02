using DatabaseProject.Consts;

namespace DatabaseProject.Models
{
    public class Product : IModel
    {
        public int Id { get; }
        public int Price { get; }
        public string Name { get; }
        public string Descr { get; }

        //All integer values in SQLite are represented as Int64(long). Thats why we need to cast
        public Product(long id, string name, long price, string descr)
        {
            Id = (int)id;
            Name = name;
            Price = (int)price;
            Descr = descr;
        }

        public override string ToString()
        {
            return $"Item ID: {Id} || Item name: {Name} || Price: {Price}{Constants.Currency} || Description: {Descr}";
        }

    }
}
