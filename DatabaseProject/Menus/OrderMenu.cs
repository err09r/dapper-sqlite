using DatabaseProject.Consts;
using DatabaseProject.Database;
using DatabaseProject.Events;
using DatabaseProject.Models;
using DatabaseProject.Utils;
using System;
using System.Data.SQLite;
using System.Linq;
using System.Threading;

namespace DatabaseProject.Menu
{
    class OrderMenu : IMenu, IMenuModelCreator
    {
        private string error = "";

        private OrderRepository repository;
        private MenuController controller;

        private event EventHandler<OrderInsertedEventArgs> OnModelInserted;
        private event EventHandler<OrderNotFoundEventArgs> OnModelNotFound;
        private event EventHandler<OrderDeletedEventArgs> OnModelDeleted;
        private event EventHandler OnModelDataInvalid;
        private event EventHandler OnModelFound;

        public OrderMenu(MenuController mController, OrderRepository orderRepository)
        {
            controller = mController;
            repository = orderRepository;
            OnModelNotFound += OnOrderNotFound;
            OnModelFound += OnOrderFound;
            OnModelInserted += OnOrderInserted;
            OnModelDeleted += OnOrderDeleted;
            OnModelDataInvalid += OnOrderDataInvalid;
        }

        public void Start()
        {
            Show(error);
            StartInputListener();
        }

        public void Show(string error)
        {
            MenuUtils.ShowOrderMenu(error);
        }

        public void StartInputListener()
        {
            do
            {
                int action = GetUserAction();
                switch (action)
                {
                    case 1:
                        {
                            Order order;
                            do
                            {
                                order = (Order)CreateModel();
                                try
                                {
                                    repository.Insert(order);
                                    OnModelInserted?.Invoke(this, new OrderInsertedEventArgs(order.Id));
                                    break;
                                }
                                catch (SQLiteException)
                                {
                                    OnModelDataInvalid?.Invoke(this, new EventArgs());
                                    break;
                                }
                            } while (true);

                            Console.ReadKey();
                            controller.GoBack();
                            break;
                        }
                    case 2:
                        {
                            Console.Clear();
                            MenuUtils.ShowFindOrderHeader();
                            string orderId = Console.ReadLine();
                            var order = repository.Find(orderId);
                            if (order != null)
                            {
                                OnModelFound?.Invoke(this, new EventArgs());
                                Console.WriteLine(order);
                            }
                            else
                            {
                                OnModelNotFound?.Invoke(this, new OrderNotFoundEventArgs(orderId));
                            }

                            Console.ReadKey();
                            controller.GoBack();
                            break;
                        }
                    case 3:
                        {
                            string orderId = GetOrderId();
                            int rows = repository.Delete(orderId);
                            if (rows == 0)
                            {
                                OnModelNotFound?.Invoke(this, new OrderNotFoundEventArgs(orderId));
                            }
                            else
                            {
                                OnModelDeleted?.Invoke(this, new OrderDeletedEventArgs(orderId));
                            }
                            Console.ReadKey();
                            controller.GoBack();
                            break;
                        }
                    case 4:
                        {
                            ShowModelList();
                            Console.ReadKey();
                            controller.GoBack();
                            break;
                        }
                    case 5:
                        {
                            controller.GoBack();
                            break;
                        }
                    default:
                        {
                            error = Constants.ErrorMsg;
                            Start();
                            break;
                        }
                }
            } while (true);
        }

        private static string GetOrderId()
        {
            Console.Clear();
            Console.WriteLine("Enter order ID of order you want to delete:\n");
            string code = Console.ReadLine();
            return code;
        }

        public IModel CreateModel()
        {
            Console.Clear();
            Console.WriteLine(" * Username: ");
            string username = Console.ReadLine().Trim();
            Console.WriteLine(" * Selected product ID: ");
            int productId = int.Parse(Console.ReadLine());
            int userId = repository.FindByUsername(username);
            return new Order(Guid.NewGuid().ToString(), DateTime.Now.ToString(Constants.DateFormatPattern), userId, productId);
        }

        public void ShowModelList()
        {
            var tokenSource = new CancellationTokenSource();
            MenuController.DisplayLoading(tokenSource);

            var task = repository.GetListAsync();
            var awaiter = task.GetAwaiter();
            awaiter.OnCompleted(() => tokenSource.Cancel());
            var list = awaiter.GetResult().ToList();

            MenuUtils.ShowList(list);
        }

        public int GetUserAction()
        {
            bool isParsed;
            int action;
            do
            {
                isParsed = int.TryParse(Console.ReadKey().KeyChar.ToString(), out action);
                if (!isParsed)
                {
                    error = Constants.ErrorMsg;
                }
                else
                {
                    error = "";
                }
                Show(error);
            } while (!isParsed);

            return action;
        }

        private static void OnOrderNotFound(object sender, OrderNotFoundEventArgs e)
        {
            Console.Clear();
            Console.WriteLine("\n * Order: {0} was not found!", e.OrderId);
        }

        private static void OnOrderFound(object sender, EventArgs e)
        {
            Console.Clear();
            Console.WriteLine("\n * Order found!");
        }

        private static void OnOrderDeleted(object sender, OrderDeletedEventArgs e)
        {
            Console.Clear();
            Console.WriteLine("\n * Order: {0} was successfully deleted!", e.OrderId);
        }

        private static void OnOrderInserted(object sender, OrderInsertedEventArgs e)
        {
            Console.Clear();
            Console.WriteLine("\n * Order: {0} was successfully registered!", e.OrderId);
        }

        private static void OnOrderDataInvalid(object sender, EventArgs e)
        {
            Console.Clear();
            Console.WriteLine("\n * User or product is invalid! Try again.");
        }

        public void Dispose()
        {
            OnModelNotFound -= OnOrderNotFound;
            OnModelFound -= OnOrderFound;
            OnModelInserted -= OnOrderInserted;
            OnModelDeleted -= OnOrderDeleted;
            OnModelDataInvalid -= OnOrderDataInvalid;
        }
    }
}
