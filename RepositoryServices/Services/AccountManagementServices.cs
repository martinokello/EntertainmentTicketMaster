using TicketMasterDataAccess.ConcreteRepositories;
using TicketMasterDataAccess.DataAccess;
using TicketMasterDataAccess.UnitOfWork;
using TicketMasterDataAccess.UnitOfWork.IUnitOfWork;

namespace RepositoryServices.Services
{
    public class AccountManagementServices : IAccontManagerServiceSegregator
    {
        private TicketMasterUserRepository _userRepository;
        private IUnitOfWork _unitOfWork;

        public AccountManagementServices(TicketMasterUserRepository userRepository)
        {
            _userRepository = userRepository;
            _unitOfWork = new UnitOfWork<TicketMasterUser>(userRepository);
            (_unitOfWork as UnitOfWork<TicketMasterUser>).DBContext = new TicketMasterEntities();
        }

        public bool ChangeUserEmail(TicketMasterUser user)
        {
            var result = _userRepository.ChangeEmail(user);
            _unitOfWork.SaveChanges();
            return result;
        }
    }

    public interface IAccontManagerServiceSegregator
    {
        
    }
}


