using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TicketMasterDataAccess.Abstracts;
using TicketMasterDataAccess.DataAccess;
using TicketMasterDataAccess.Factories;
using TicketMasterDataAccess.UnitOfWork.IUnitOfWork;

namespace TicketMasterDataAccess.ConcreteRepositories
{
    public class TicketMasterUserRepository : AbstractTicketRepository<TicketMasterUser, int>, ITicketMasterUserRepositorySegregator
    { 
        public TicketMasterUserRepository()
        {
        }

        public bool ChangeEmail(TicketMasterUser user)
        {
            try
            {
                var dbUser = GetUserByName(user.UserName);
                dbUser.Email = user.Email;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
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
                    DBContext.TicketMasterUsers.Remove(
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
                    user.AspNetUser = instance.AspNetUser;
                    user.Email = instance.Email;
                user.UserName = instance.UserName;
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        public TicketMasterUser GetUserByName(string username)
        {
            return DBContext.TicketMasterUsers.SingleOrDefault(p => p.UserName == username);
        }
    }

    public interface ITicketMasterUserRepositorySegregator
    {

    }
}
