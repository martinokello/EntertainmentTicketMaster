using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketMasterDataAccess.Interfaces
{
    public interface IRepository<T,TKey>
    {
        T GetById(TKey key);
        T[] GetAll();
        bool Add(T instance);
        bool Delete(TKey key);
        bool Update(T instance);

    }
}
