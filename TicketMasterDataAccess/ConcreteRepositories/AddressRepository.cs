using System;
using System.Linq;
using System.Runtime.InteropServices;
using TicketMasterDataAccess.Abstracts;
using TicketMasterDataAccess.DataAccess;
using TicketMasterDataAccess.Factories;
using TicketMasterDataAccess.Interfaces;
using TicketMasterDataAccess.UnitOfWork.IUnitOfWork;

namespace TicketMasterDataAccess.ConcreteRepositories
{
    public class EntertainmentAddressRepository : AbstractTicketRepository<EntertainmentAddress, int>, IEntertainmentAddressRepositorySegregator
    {
        public EntertainmentAddressRepository()
        {
            
        }

        public override bool Add(EntertainmentAddress instance)
        {
            var result = base.Add(instance);
            return result;
        }
        public override EntertainmentAddress GetById(int key)
        {
            return DBContext.EntertainmentAddresses.SingleOrDefault(p => p.UserId == key);
        }

        public override bool Delete(int key)
        {
            try
            {
                DBContextFactory.GetDbContextInstance()
                    .EntertainmentAddresses.Remove(
                        DBContext.EntertainmentAddresses.SingleOrDefault(k => k.AddressId == key));
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public override bool Update(EntertainmentAddress instance)
        {
            try
            {
                var EntertainmentAddress =
                    DBContext.EntertainmentAddresses.SingleOrDefault(k => k.UserId == instance.UserId);
                EntertainmentAddress.AddressLine1 = instance.AddressLine1;
                EntertainmentAddress.AddressLine2 = instance.AddressLine2;
                EntertainmentAddress.Town = instance.Town;
                EntertainmentAddress.Country = instance.Country;
                EntertainmentAddress.PostCode = instance.PostCode;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public virtual EntertainmentAddress GetEntertainmentAddressByUsername(string username)
        {
            try
            {
                var user =
                    DBContext.TicketMasterUsers.SingleOrDefault(p => p.UserName == username);

                if (user != null)
                {
                    var EntertainmentAddress = DBContext.EntertainmentAddresses.SingleOrDefault(p => p.UserId == user.UserId);
                    return EntertainmentAddress;
                }
            }
            catch (Exception e)
            {

            }
            return null;
        }
    }

    public interface IEntertainmentAddressRepositorySegregator
    {
        
    }
}

