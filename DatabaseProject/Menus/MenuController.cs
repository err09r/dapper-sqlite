using DatabaseProject.Consts;
using DatabaseProject.Database;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DatabaseProject.Menu
{
    class MenuController
    {
        private UserRepository userRepository;
        private ProductRepository productRepository;
        private OrderRepository orderRepository;

        private MainMenu mainMenu;

        public MenuController()
        {
            userRepository = UserRepository.GetInstance();
            productRepository = ProductRepository.GetInstance();
            orderRepository = OrderRepository.GetInstance();

            var userMenu = new UserMenu(this, userRepository);
            var productMenu = new ProductMenu(this, productRepository);
            var orderMenu = new OrderMenu(this, orderRepository);
            mainMenu = new MainMenu(this, userMenu, productMenu, orderMenu);
        }

        public void Launch()
        {
            mainMenu.Start();
        }

        public void GoBack()
        {
            Launch();
        }

        public void Exit()
        {
            mainMenu.Dispose();
            Environment.Exit(0);
        }

        public static void DisplayLoading(CancellationTokenSource tokenSource)
        {
            string msg = "Loading data";
            var token = tokenSource.Token;

            Task.Factory.StartNew(() =>
            {
                StringBuilder loadingText = new(msg);
                int i = 0;
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine(loadingText.ToString());
                    Thread.Sleep(Constants.UpdateFrequency);
                    i++;

                    if (i <= 3)
                    {
                        loadingText.Append('.');
                    }
                    else
                    {
                        loadingText.Clear();
                        loadingText.Append(msg);
                        i = 0;
                    }

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }, token);
        }
    }
}
