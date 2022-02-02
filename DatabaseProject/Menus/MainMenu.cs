using DatabaseProject.Consts;
using DatabaseProject.Utils;
using System;

namespace DatabaseProject.Menu
{
    class MainMenu : IMenu
    {
        private UserMenu userMenu;
        private ProductMenu productMenu;
        private OrderMenu orderMenu;

        private MenuController controller;

        private string error = "";

        public MainMenu(MenuController mController, params IMenu[] menus)
        {
            controller = mController;
            userMenu = (UserMenu)menus[0];
            productMenu = (ProductMenu)menus[1];
            orderMenu = (OrderMenu)menus[2];
        }

        public void Start()
        {
            Show(error);
            StartInputListener();
        }

        public void Show(string error)
        {
            MenuUtils.ShowMainMenu(error);
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
                            userMenu.Start();
                            break;
                        }
                    case 2:
                        {
                            productMenu.Start();
                            break;
                        }
                    case 3:
                        {
                            orderMenu.Start();
                            break;
                        }
                    case 4:
                        {
                            controller.Exit();
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

        public void Dispose()
        {
            userMenu.Dispose();
            productMenu.Dispose();
            orderMenu.Dispose();
            Console.Clear();
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
    }
}
