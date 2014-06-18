using System;
using System.Collections.Generic;
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
    public class EventRepository : AbstractTicketRepository<Event, int>, IEventRepositorySegregator
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public override bool Add(Event instance)
        {
            var result = base.Add(instance);
            _unitOfWork.SaveChanges();
            return result;
        }
        public override Event GetById(int key)
        {
            return DBContextFactory.GetDbContextInstance().Events.SingleOrDefault(p => p.EventId == key);
        }

        public override bool Delete(int key)
        {
                try
                {
                    DBContextFactory.GetDbContextInstance()
                        .Events.Remove(
                            DBContextFactory.GetDbContextInstance().Events.SingleOrDefault(k => k.EventId == key));
                    _unitOfWork.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                }
                return false;
        }
        public override bool Update(Event instance)
        {
                try
                {
                    var evt =
                        DBContextFactory.GetDbContextInstance()
                            .Events.SingleOrDefault(k => k.EventId == instance.EventId);
                    evt.EventDescription = instance.EventDescription;
                    evt.EventId = instance.EventId;
                    evt.NumberOfTickets = instance.NumberOfTickets;
                    evt.PricePerTicket = instance.PricePerTicket;
                    evt.EventName = instance.EventName;
                    evt.Location = instance.Location;
                    evt.Tickets = instance.Tickets;
                    evt.EventDate = instance.EventDate;
                    _unitOfWork.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
        }

        public decimal? GetUnitPriceOfTicketByEventId(int eventId)
        {
            return
                DBContextFactory.GetDbContextInstance().Events.SingleOrDefault(p => p.EventId == eventId).PricePerTicket;
        }
    }
    public interface IEventRepositorySegregator
    {

    }
}
