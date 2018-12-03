using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketMasterDataAccess.DataAccess;
using TicketMasterDataAccess.Factories;
using TicketMasterDataAccess.Interfaces;

namespace TicketMasterDataAccess.Abstracts
{
    public abstract class AbstractTicketRepository<T,TKey> :IRepository<T,TKey> where T : class
    {
        public abstract T GetById(TKey key);

        public virtual T[] GetAll()
        {
            return DBContextFactory.GetDbContextInstance().Set<T>().ToArray<T>();
        }

        public virtual bool Add(T instance)
        {
            try
            {
                DBContextFactory.GetDbContextInstance().Set<T>().Add(instance);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public abstract bool Delete(TKey key);

        public abstract bool Update(T instance);
    }
}
