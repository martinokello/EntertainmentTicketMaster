using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketMasterDataAccess.DataAccess;
using TicketMasterDataAccess.Factories;
using TicketMasterDataAccess.Abstracts;

namespace TicketMasterDataAccess.UnitOfWork
{
    public class UnitOfWork<T>: IUnitOfWork.IUnitOfWork where T : class
    {
        public TicketMasterEntities DBContext { get; set; }
        private AbstractTicketRepository<T, int> repository;
        public UnitOfWork(AbstractTicketRepository<T,int> repository)
        {
            this.repository = repository;
        }
        public void SaveChanges()
        {
            DBContext.SaveChanges();
        }
    }
}
