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
    class UserMenu : IMenu, IMenuModelCreator
    {
        private string error = "";

        private UserRepository repository;
        private MenuController controller;

        private event EventHandler<UserInsertedEventArgs> OnModelInserted;
        private event EventHandler<UserNotFoundEventArgs> OnModelNotFound;
        private event EventHandler<UserExistsEventArgs> OnModelExists;
        private event EventHandler<UserDeletedEventArgs> OnModelDeleted;
        private event EventHandler<UserUpdatedEventArgs> OnModelUpdated;

        public UserMenu(MenuController mController, UserRepository userRepository)
        {
            controller = mController;
            repository = userRepository;
            OnModelNotFound += OnUserNotFound;
            OnModelUpdated += OnUserUpdated;
            OnModelInserted += OnUserInserted;
            OnModelDeleted += OnUserDeleted;
            OnModelExists += OnUserExists;
        }

        public void Start()
        {
            Show(error);
            StartInputListener();
        }

        public void Show(string error)
        {
            MenuUtils.ShowUserMenu(error);
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
                            User user;
                            do
                            {
                                user = (User)CreateModel();
                                try
                                {
                                    repository.Insert(user);
                                }
                                catch (SQLiteException)
                                {
                                    OnModelExists?.Invoke(this, new UserExistsEventArgs(user.Username));
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
                                OnModelInserted?.Invoke(this, new UserInsertedEventArgs(user.Username));
                                Console.ReadKey();
                            }
                            controller.GoBack();
                            break;
                        }
                    case 2:
                        {
                            string name = GetUsername();
                            bool wasAdded = true;
                            try
                            {
                                int rows = repository.Delete(name);
                                if (rows == 0)
                                {
                                    OnModelNotFound?.Invoke(this, new UserNotFoundEventArgs(name));
                                    wasAdded = false;
                                }
                            }
                            catch (SQLiteException)
                            {
                                OnModelNotFound?.Invoke(this, new UserNotFoundEventArgs(name));
                                wasAdded = false;
                            }
                            if (wasAdded)
                            {
                                OnModelDeleted?.Invoke(this, new UserDeletedEventArgs(name));
                            }
                            Console.ReadKey();
                            controller.GoBack();
                            break;
                        }
                    case 3:
                        {
                            string oldUsername = GetOldUsername();
                            string newUsername = GetNewUsername();
                            bool wasAdded = true;
                            try
                            {
                                int rows = repository.Update(oldUsername, newUsername);
                                if (rows == 0)
                                {
                                    OnModelNotFound?.Invoke(this, new UserNotFoundEventArgs(oldUsername));
                                    wasAdded = false;
                                }

                            }
                            catch (SQLiteException)
                            {
                                OnModelNotFound?.Invoke(this, new UserNotFoundEventArgs(oldUsername));
                                wasAdded = false;
                            }
                            if (wasAdded)
                            {
                                OnModelUpdated?.Invoke(this, new UserUpdatedEventArgs(oldUsername, newUsername));
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
            Console.WriteLine(" * Enter username: ");
            string username = Console.ReadLine();
            Console.WriteLine(" * Enter email: ");
            string email = Console.ReadLine();
            Console.WriteLine(" * Enter address: ");
            string address = Console.ReadLine();

            //User ID set to 0 due to native ID auto-incrementation in SQLite. Can be set to any int
            return new User(0, username, DateTime.Now.ToString(Constants.DateFormatPattern), email, address);
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

        private static string GetOldUsername()
        {
            Console.Clear();
            Console.WriteLine("Enter username of user you want to change:\n");
            string name = Console.ReadLine();
            return name;
        }

        private static string GetNewUsername()
        {
            Console.Clear();
            Console.WriteLine("Enter new username:\n");
            return Console.ReadLine();
        }

        private static string GetUsername()
        {
            Console.Clear();
            Console.WriteLine("Enter username of user you want to delete:\n");
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

        private static void OnUserNotFound(object sender, UserNotFoundEventArgs e)
        {
            Console.Clear();
            Console.WriteLine("\n * Order: {0} was not found!", e.Username);
        }

        private static void OnUserExists(object sender, UserExistsEventArgs e)
        {
            Console.Clear();
            Console.WriteLine("\n* User: {0} already exists!\nDo you want to try again? (Y/N)", e.Username);
        }

        private static void OnUserUpdated(object sender, UserUpdatedEventArgs e)
        {
            Console.Clear();
            Console.WriteLine("\n * Username of user: {0} was updated to: {1}", e.OldUsername, e.NewUsername);
        }

        private static void OnUserDeleted(object sender, UserDeletedEventArgs e)
        {
            Console.Clear();
            Console.WriteLine("\n * Order: {0} was successfully deleted!", e.Username);
        }

        private static void OnUserInserted(object sender, UserInsertedEventArgs e)
        {
            Console.Clear();
            Console.WriteLine("\n * User: {0} was successfully registered!", e.Username);
        }

        public void Dispose()
        {
            OnModelNotFound -= OnUserNotFound;
            OnModelNotFound -= OnUserNotFound;
            OnModelInserted -= OnUserInserted;
            OnModelDeleted -= OnUserDeleted;
            OnModelExists -= OnUserExists;
        }
    }
}
