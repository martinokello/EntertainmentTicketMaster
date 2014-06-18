using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TicketMasterDataAccess.Abstracts;
using TicketMasterDataAccess.DataAccess;
using TicketMasterDataAccess.Factories;
using TicketMasterDataAccess.UnitOfWork;
using TicketMasterDataAccess.UnitOfWork.IUnitOfWork;

namespace TicketMasterDataAccess.ConcreteRepositories
{
    public class TicketRepository : AbstractTicketRepository<Ticket, int>, ITicketRepositorySegregator
    {
        private readonly IUnitOfWork _unitOfWork;

        public TicketRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public override Ticket GetById(int key)
        {
            return DBContextFactory.GetDbContextInstance().Tickets.SingleOrDefault(p => p.TicketId == key);
        }
        public override bool Add(Ticket instance)
        {
            var result = base.Add(instance);
            _unitOfWork.SaveChanges();
            return result;
        }
        public override bool Delete(int key)
        {
                try
                {
                    DBContextFactory.GetDbContextInstance()
                        .Tickets.Remove(
                            DBContextFactory.GetDbContextInstance().Tickets.SingleOrDefault(k => k.TicketId == key));
                    _unitOfWork.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
        }

        public override bool Update(Ticket instance)
        {
                try
                {
                    var ticket =
                        DBContextFactory.GetDbContextInstance()
                            .Tickets.SingleOrDefault(k => k.TicketId == instance.TicketId);
                    ticket.EventId = instance.EventId;
                    ticket.Price = instance.Price;
                    ticket.TicketGUID = instance.TicketGUID;
                    _unitOfWork.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
        }
    }
    public interface ITicketRepositorySegregator
    {

    }
}
