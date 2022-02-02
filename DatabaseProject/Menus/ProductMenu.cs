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
    class ProductMenu : IMenu, IMenuModelCreator
    {
        private string error = "";

        private ProductRepository repository;
        private MenuController controller;

        private event EventHandler<ProductInsertedEventArgs> OnModelInserted;
        private event EventHandler<ProductNotFoundEventArgs> OnModelNotFound;
        private event EventHandler<ProductDeletedEventArgs> OnModelDeleted;
        private event EventHandler<ProductUpdatedEventArgs> OnModelUpdated;
        private event EventHandler<ProductExistsEventArgs> OnModelExists;

        public ProductMenu(MenuController mController, ProductRepository productRepository)
        {
            controller = mController;
            repository = productRepository;
            OnModelNotFound += OnProductNotFound;
            OnModelUpdated += OnProductUpdated;
            OnModelInserted += OnProductInserted;
            OnModelDeleted += OnProductDeleted;
            OnModelExists += OnProductExists;
        }

        public void Start()
        {
            Show(error);
            StartInputListener();
        }

        public void Show(string error)
        {
            MenuUtils.ShowItemMenu(error);
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
                            bool isFinished = true;
                            bool userExists = false;
                            Product product;
                            do
                            {
                                product = (Product)CreateModel();
                                try
                                {
                                    repository.Insert(product);
                                }
                                catch (SQLiteException)
                                {
                                    OnModelExists?.Invoke(this, new ProductExistsEventArgs(product.Name));
                                    userExists = true;
                                    var input = Console.ReadKey();
                                    if (input.KeyChar == 'Y' || input.KeyChar == 'y')
                                    {
                                        isFinished = false;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            } while (!isFinished);

                            if (!userExists)
                            {
                                OnModelInserted?.Invoke(this, new ProductInsertedEventArgs(product.Name));
                                Console.ReadKey();
                            }
                            controller.GoBack();
                            break;
                        }
                    case 2:
                        {
                            string name = GetProductName();
                            bool wasAdded = true;
                            try
                            {
                                int rows = repository.Delete(name);
                                if (rows == 0)
                                {
                                    OnModelNotFound?.Invoke(this, new ProductNotFoundEventArgs(name));
                                    wasAdded = false;
                                }
                            }
                            catch (SQLiteException)
                            {
                                OnModelNotFound?.Invoke(this, new ProductNotFoundEventArgs(name));
                                wasAdded = false;
                            }
                            if (wasAdded)
                            {
                                OnModelDeleted?.Invoke(this, new ProductDeletedEventArgs(name));
                            }
                            Console.ReadKey();
                            controller.GoBack();
                            break;
                        }
                    case 3:
                        {
                            string oldUsername = GetOldProductName();
                            string newUsername = GetNewProductName();
                            bool wasAdded = true;
                            try
                            {
                                int rows = repository.Update(oldUsername, newUsername);
                                if (rows == 0)
                                {
                                    OnModelNotFound?.Invoke(this, new ProductNotFoundEventArgs(oldUsername));
                                    wasAdded = false;
                                }
                            }
                            catch (SQLiteException)
                            {
                                OnModelNotFound?.Invoke(this, new ProductNotFoundEventArgs(oldUsername));
                                wasAdded = false;
                            }
                            if (wasAdded)
                            {
                                OnModelUpdated?.Invoke(this, new ProductUpdatedEventArgs(oldUsername, newUsername));
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

        public IModel CreateModel()
        {
            Console.Clear();
            Console.WriteLine(" * Enter product name: ");
            string name = Console.ReadLine();
            Console.WriteLine(" * Enter product price: ");
            int price = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine(" * Enter product description: ");
            string descr = Console.ReadLine();

            //Item ID set to 0 due to native ID auto-incrementation in SQLite. Can be set to any int
            return new Product(0, name, price, descr);
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

        private static string GetOldProductName()
        {
            Console.Clear();
            Console.WriteLine("Enter name of product you want to change:\n");
            return Console.ReadLine();
        }

        private static string GetNewProductName()
        {
            Console.Clear();
            Console.WriteLine("Enter new product name:\n");
            return Console.ReadLine();
        }

        private static string GetProductName()
        {
            Console.Clear();
            Console.WriteLine("Enter name of product you want to delete:\n");
            return Console.ReadLine();
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

        private static void OnProductNotFound(object sender, ProductNotFoundEventArgs e)
        {
            Console.Clear();
            Console.WriteLine("\n * Product: {0} was not found!", e.ProductName);
        }

        private static void OnProductUpdated(object sender, ProductUpdatedEventArgs e)
        {
            Console.Clear();
            Console.WriteLine("\n * Name of product: {0} was updated to: {1}", e.OldProductName, e.NewProductName);
        }

        private static void OnProductDeleted(object sender, ProductDeletedEventArgs e)
        {
            Console.Clear();
            Console.WriteLine("\n * Product: {0} was successfully deleted!", e.ProductName);
        }

        private static void OnProductInserted(object sender, ProductInsertedEventArgs e)
        {
            Console.Clear();
            Console.WriteLine("\n * Product: {0} was successfully registered!", e.ProductName);
        }

        private static void OnProductExists(object sender, ProductExistsEventArgs e)
        {
            Console.Clear();
            Console.WriteLine("\n * Product: {0} already exists!\nDo you want to try again? (Y/N)", e.ProductName);
        }

        public void Dispose()
        {
            OnModelNotFound -= OnProductNotFound;
            OnModelUpdated -= OnProductUpdated;
            OnModelInserted -= OnProductInserted;
            OnModelDeleted -= OnProductDeleted;
            OnModelExists -= OnProductExists;
        }
    }
}
