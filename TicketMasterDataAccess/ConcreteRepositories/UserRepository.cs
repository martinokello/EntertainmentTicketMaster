using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TicketMasterDataAccess.Abstracts;
using TicketMasterDataAccess.DataAccess;
using TicketMasterDataAccess.Factories;
using TicketMasterDataAccess.Interfaces;
using TicketMasterDataAccess.UnitOfWork.IUnitOfWork;

namespace TicketMasterDataAccess.ConcreteRepositories
{
    public class UserRepository : AbstractTicketRepository<TicketMasterUser, int>, IUserRepositorySegregator
    {
        public UserRepository()
        {
            
        }

        public override bool Add(TicketMasterUser instance)
        {
            var result = base.Add(instance);
            return result;
        }
        public override TicketMasterUser GetById(int key)
        {
            return DBContext.TicketMasterUsers.SingleOrDefault(p => p.UserId == key);
        }

        public override bool Delete(int key)
        {
                try
                {
                    DBContextFactory.GetDbContextInstance()
                        .TicketMasterUsers.Remove(
                            DBContext.TicketMasterUsers.SingleOrDefault(k => k.UserId == key));
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
        }

        public override bool Update(TicketMasterUser instance)
        {
                try
                {
                    var user =
                        DBContext.TicketMasterUsers.SingleOrDefault(k => k.UserId == instance.UserId);
                    user.UserName = instance.UserName;
                    user.Email = instance.Email;
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

    }
    public interface IUserRepositorySegregator
    {

    }
}
