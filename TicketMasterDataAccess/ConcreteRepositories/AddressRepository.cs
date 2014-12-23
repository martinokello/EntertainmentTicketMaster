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
        private readonly IUnitOfWork _unitOfWork;

        public EntertainmentAddressRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override bool Add(EntertainmentAddress instance)
        {
            var result = base.Add(instance);
            _unitOfWork.SaveChanges();
            return result;
        }
        public override EntertainmentAddress GetById(int key)
        {
            return DBContextFactory.GetDbContextInstance().EntertainmentAddresses.SingleOrDefault(p => p.UserId == key);
        }

        public override bool Delete(int key)
        {
            try
            {
                DBContextFactory.GetDbContextInstance()
                    .EntertainmentAddresses.Remove(
                        DBContextFactory.GetDbContextInstance().EntertainmentAddresses.SingleOrDefault(k => k.AddressId == key));
                _unitOfWork.SaveChanges();
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
                    DBContextFactory.GetDbContextInstance()
                        .EntertainmentAddresses.SingleOrDefault(k => k.UserId == instance.UserId);
                EntertainmentAddress.AddressLine1 = instance.AddressLine1;
                EntertainmentAddress.AddressLine2 = instance.AddressLine2;
                EntertainmentAddress.Town = instance.Town;
                EntertainmentAddress.Country = instance.Country;
                EntertainmentAddress.PostCode = instance.PostCode;
                _unitOfWork.SaveChanges();
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
                    DBContextFactory.GetDbContextInstance()
                        .TicketMasterUsers.SingleOrDefault(p => p.UserName == username);

                if (user != null)
                {
                    var EntertainmentAddress = DBContextFactory.GetDbContextInstance().EntertainmentAddresses.SingleOrDefault(p => p.UserId == user.UserId);
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

