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
        private readonly IUnitOfWork _unitOfWork;

        public TicketMasterUserRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool ChangeEmail(TicketMasterUser user)
        {
            try
            {
                var dbUser = GetUserByName(user.UserName);
                dbUser.Email = user.Email;
                _unitOfWork.SaveChanges();
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
            _unitOfWork.SaveChanges();
            return result;
        }

        public override TicketMasterUser GetById(int key)
        {
            return DBContextFactory.GetDbContextInstance().TicketMasterUsers.SingleOrDefault(p => p.UserId == key);
        }


        public override bool Delete(int key)
        {
                try
                {
                    DBContextFactory.GetDbContextInstance()
                        .TicketMasterUsers.Remove(
                            DBContextFactory.GetDbContextInstance()
                                .TicketMasterUsers.SingleOrDefault(k => k.UserId == key));
                    _unitOfWork.SaveChanges();
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
                        DBContextFactory.GetDbContextInstance()
                            .TicketMasterUsers.SingleOrDefault(k => k.UserId == instance.UserId);
                    user.AspNetUser = instance.AspNetUser;
                    user.Email = instance.Email;
                    user.UserName = instance.UserName;
                    _unitOfWork.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        public virtual TicketMasterUser GetUserByName(string username)
        {
            return DBContextFactory.GetDbContextInstance().TicketMasterUsers.SingleOrDefault(p => p.UserName == username);
        }
    }

    public interface ITicketMasterUserRepositorySegregator
    {

    }
}
