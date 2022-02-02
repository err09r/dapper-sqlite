using System;

namespace DatabaseProject.Events
{
    public class UserNotFoundEventArgs : EventArgs
    {
        public string Username { get; }

        public UserNotFoundEventArgs(string username)
        {
            Username = username;
        }
    }

    public class UserInsertedEventArgs : EventArgs
    {
        public string Username { get; }

        public UserInsertedEventArgs(string username)
        {
            Username = username;
        }
    }
    
    public class UserDeletedEventArgs : EventArgs
    {
        public string Username { get; }

        public UserDeletedEventArgs(string username)
        {
            Username = username;
        }
    }
    
    public class UserUpdatedEventArgs : EventArgs
    {
        public string OldUsername { get; }
        public string NewUsername { get; }

        public UserUpdatedEventArgs(string oldUsername, string newUsername)
        {
            OldUsername = oldUsername;
            NewUsername = newUsername;
        }
    }

    public class UserExistsEventArgs : EventArgs
    {
        public string Username { get; }

        public UserExistsEventArgs(string username)
        {
            Username = username;
        }
    }

    public class ProductExistsEventArgs : EventArgs
    {
        public string ProductName { get; }

        public ProductExistsEventArgs(string productName)
        {
            ProductName = productName;
        }
    }

    public class ProductNotFoundEventArgs : EventArgs
    {
        public string ProductName { get; }

        public ProductNotFoundEventArgs(string productName)
        {
            ProductName = productName;
        }
    }

    public class ProductUpdatedEventArgs : EventArgs
    {
        public string OldProductName { get; }
        public string NewProductName { get; }

        public ProductUpdatedEventArgs(string oldProductName, string newProductName)
        {
            OldProductName = oldProductName;
            NewProductName = newProductName;
        }
    }

    public class ProductDeletedEventArgs : EventArgs
    {
        public string ProductName { get; }

        public ProductDeletedEventArgs(string productName)
        {
            ProductName = productName;
        }
    }

    public class ProductInsertedEventArgs : EventArgs
    {
        public string ProductName { get; }

        public ProductInsertedEventArgs(string productName)
        {
            ProductName = productName;
        }
    }

    public class OrderInsertedEventArgs : EventArgs
    {
        public string OrderId { get; }

        public OrderInsertedEventArgs(string orderId)
        {
            OrderId = orderId;
        }
    }

    public class OrderDeletedEventArgs : EventArgs
    {
        public string OrderId { get; }

        public OrderDeletedEventArgs(string orderId)
        {
            OrderId = orderId;
        }
    }

    public class OrderNotFoundEventArgs : EventArgs
    {
        public string OrderId { get; }

        public OrderNotFoundEventArgs(string orderId)
        {
            OrderId = orderId;
        }
    }
}
