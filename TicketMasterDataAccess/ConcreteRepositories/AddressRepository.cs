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
    public class AddressRepository : AbstractTicketRepository<Address, int>, IAddressRepositorySegregator
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddressRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override bool Add(Address instance)
        {
            var result = base.Add(instance);
            _unitOfWork.SaveChanges();
            return result;
        }
        public override Address GetById(int key)
        {
            return DBContextFactory.GetDbContextInstance().Addresses.SingleOrDefault(p => p.UserId == key);
        }

        public override bool Delete(int key)
        {
            try
            {
                DBContextFactory.GetDbContextInstance()
                    .Addresses.Remove(
                        DBContextFactory.GetDbContextInstance().Addresses.SingleOrDefault(k => k.AddressId == key));
                _unitOfWork.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public override bool Update(Address instance)
        {
            try
            {
                var address =
                    DBContextFactory.GetDbContextInstance()
                        .Addresses.SingleOrDefault(k => k.UserId == instance.UserId);
                address.AddressLine1 = instance.AddressLine1;
                address.AddressLine2 = instance.AddressLine2;
                address.Town = instance.Town;
                address.Country = instance.Country;
                address.PostCode = instance.PostCode;
                _unitOfWork.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public virtual Address GetAddressByUsername(string username)
        {
            try
            {
                var user =
                    DBContextFactory.GetDbContextInstance()
                        .TicketMasterUsers.SingleOrDefault(p => p.UserName == username);

                if (user != null)
                {
                    var address = DBContextFactory.GetDbContextInstance().Addresses.SingleOrDefault(p => p.UserId == user.UserId);
                    return address;
                }
            }
            catch (Exception e)
            {

            }
            return null;
        }
    }

    public interface IAddressRepositorySegregator
    {
        
    }
}

