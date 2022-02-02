using DatabaseProject.Menu;

namespace DatabaseProject
{
    class Program
    {
        static void Main(string[] args)
        {
            var menuController = new MenuController();
            menuController.Launch();
        }
    }
}