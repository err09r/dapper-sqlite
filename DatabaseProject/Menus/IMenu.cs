using System;

namespace DatabaseProject.Menu
{
    interface IMenu : IDisposable
    {
        void Start();
        void Show(string error);
        void StartInputListener();
        int GetUserAction();
    }
}
