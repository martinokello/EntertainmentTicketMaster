using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketMasterDataAccess.DataAccess;
using System.Web;

namespace TicketMasterDataAccess.Factories
{
    public class DBContextFactory
    {
        private static TicketMasterEntities _dbContext;
        private static readonly object LockOjbect = new object();
        public static TicketMasterEntities GetDbContextInstance()
        {
            _dbContext = HttpContext.Current.Application.Get("DBContextObject") != null
                   ? (TicketMasterEntities)HttpContext.Current.Application.Get("DBContextObject")
                   : null;

            if (_dbContext == null)
            {
                _dbContext = new TicketMasterEntities();

                HttpContext.Current.Application.Set("DBContextObject", _dbContext);
            }

            return _dbContext;
        }
    }
}
