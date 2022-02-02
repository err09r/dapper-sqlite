using DatabaseProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatabaseProject.Database
{
    interface IRepository<T> where T : IModel
    {
        void Insert(T model);

        int Update(string oldName, string newName);

        int Delete(string name);

        //IEnumerable<T> GetList(); //Replaced by async method with long-running operation simulation

        Task<IEnumerable<T>> GetListAsync();
    }
}
