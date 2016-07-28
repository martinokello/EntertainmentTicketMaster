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
        public TicketRepository(TicketMasterEntities context):base(context)
        {
        }
        public override Ticket GetById(int key)
        {
            return DBContext.Tickets.SingleOrDefault(p => p.TicketId == key);
        }
        public override bool Add(Ticket instance)
        {
            var result = base.Add(instance);
            return result;
        }
        public override bool Delete(int key)
        {
                try
                {
                DBContext.Tickets.Remove(
                        DBContext.Tickets.SingleOrDefault(k => k.TicketId == key));
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
                        DBContext.Tickets.SingleOrDefault(k => k.TicketId == instance.TicketId);
                    ticket.EventId = instance.EventId;
                    ticket.Price = instance.Price;
                    ticket.TicketGUID = instance.TicketGUID;
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
