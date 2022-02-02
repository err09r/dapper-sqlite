using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseProject.Utils
{
    public static class MenuUtils
    {
        public static void ShowList<IModel>(List<IModel> list)
        {
            Console.Clear();
            var modelTitle = typeof(IModel).Name.ToUpper();
            if (list.Any())
            {
                Console.WriteLine($"_____ [LIST OF REGISTERED {modelTitle}S] _____\n");
                int i = 1;
                foreach (IModel model in list)
                {
                    Console.WriteLine($"{i}) {model}\n");
                    i++;
                }
            }
            else
            {
                Console.WriteLine($"List of {modelTitle}S is empty!");
            }
        }

        public static void ShowMainMenu(string error)
        {
            Console.Clear();

            StringBuilder sb = new();
            sb.AppendLine("_____ [MAIN MENU] _____");
            sb.AppendLine();
            sb.AppendLine("1. User panel");
            sb.AppendLine("2. Product panel");
            sb.AppendLine("3. Order panel");
            sb.AppendLine("4. Exit");

            if (error != "")
            {
                sb.AppendLine();
                sb.AppendLine(error);
            }

            Console.WriteLine(sb.ToString());
        }

        public static void ShowUserMenu(string error)
        {
            Console.Clear();

            StringBuilder sb = new();
            sb.AppendLine("_____ [USER PANEL] _____");
            sb.AppendLine();
            sb.AppendLine("1. Create user");
            sb.AppendLine("2. Delete user");
            sb.AppendLine("3. Update username");
            sb.AppendLine("4. Show user list");
            sb.AppendLine("5. Back");

            if (error != "")
            {
                sb.AppendLine();
                sb.AppendLine(error);
            }

            Console.WriteLine(sb.ToString());
        }

        public static void ShowItemMenu(string error)
        {
            Console.Clear();

            StringBuilder sb = new();
            sb.AppendLine("_____ [PRODUCT PANEL] _____");
            sb.AppendLine();
            sb.AppendLine("1. Create product");
            sb.AppendLine("2. Delete product");
            sb.AppendLine("3. Update product name");
            sb.AppendLine("4. Show product list");
            sb.AppendLine("5. Back");

            if (error != "")
            {
                sb.AppendLine();
                sb.AppendLine(error);
            }

            Console.WriteLine(sb.ToString());
        }

        public static void ShowOrderMenu(string error)
        {
            Console.Clear();

            StringBuilder sb = new();
            sb.AppendLine("_____ [ORDER PANEL] _____");
            sb.AppendLine();
            sb.AppendLine("1. Make order");
            sb.AppendLine("2. Find order");
            sb.AppendLine("3. Delete order");
            sb.AppendLine("4. Show order history");
            sb.AppendLine("5. Back");

            if (error != "")
            {
                sb.AppendLine();
                sb.AppendLine(error);
            }

            Console.WriteLine(sb.ToString());
        }

        public static void ShowFindOrderHeader()
        {
            Console.Clear();
            Console.WriteLine("Enter ID of order: ");
        }

    }
}
