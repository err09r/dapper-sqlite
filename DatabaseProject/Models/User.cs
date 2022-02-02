namespace DatabaseProject.Models
{
    public class User : IModel
    {
        public int Id { get; }
        public string Username { get; }
        public string RegDate { get; }
        public string Email { get; }
        public string Address { get; }
        //public Gender Gender { get; }

        //All integer values in SQLite are represented as Int64(long). Thats why we need to cast
        public User(long id, string username, string regDate, string email, string address)
        {
            Id = (int)id;
            Username = username;
            RegDate = regDate;
            Email = email;
            Address = address;
        }

        public override string ToString()
        {
            return $"User ID: {Id} || Username: {Username} || Registration date: {RegDate} || Email: {Email} || Address: {Address}";
        }

    }

    public enum Gender
    {
        MALE,
        FEMALE
    }
}
