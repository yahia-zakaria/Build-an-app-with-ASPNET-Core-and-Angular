using System.Threading.Tasks;
using API.Interfaces;
using AutoMapper;

namespace API.Data.Repos
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UnitOfWork(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;

        }
        public IMessageRepository MessageRepository => new MessaageRepository(_context, _mapper);

        public ILikesRepository LikesRepository => new LikesRepository(_context);

        public IUserRepository UserRepository => new UserRepository(_context, _mapper);

        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChange()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}