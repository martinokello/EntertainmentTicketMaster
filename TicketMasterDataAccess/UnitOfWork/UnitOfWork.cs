using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketMasterDataAccess.DataAccess;
using TicketMasterDataAccess.Factories;
using TicketMasterDataAccess.UnitOfWork.IUnitOfWork;

namespace TicketMasterDataAccess.UnitOfWork
{
    public class UnitOfWork: IUnitOfWork.IUnitOfWork
    {
        public void SaveChanges()
        {
            DBContextFactory.GetDbContextInstance().SaveChanges();
        }
    }
}
