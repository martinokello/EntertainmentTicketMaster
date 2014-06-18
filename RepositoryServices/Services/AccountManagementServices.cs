using TicketMasterDataAccess.ConcreteRepositories;
using TicketMasterDataAccess.DataAccess;
using TicketMasterDataAccess.UnitOfWork;
using TicketMasterDataAccess.UnitOfWork.IUnitOfWork;

namespace RepositoryServices.Services
{
    public class AccountManagementServices : IAccontManagerServiceSegregator
    {
        TicketMasterUserRepository _userRepository;

        public AccountManagementServices(ITicketRepositorySegregator ticketMasterUserRepository)
        {
            _userRepository = ticketMasterUserRepository as TicketMasterUserRepository;
        }

        public bool ChangeUserEmail(TicketMasterUser user)
        {
            return _userRepository.ChangeEmail(user);
        }
    }

    public interface IAccontManagerServiceSegregator
    {
        
    }
}


