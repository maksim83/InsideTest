using Common.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Ms1.Api.Store.Services
{
    public interface IDbService
    {
        public Task AddMessage(Message message);
        public Task<int> GetMessageCount(long sessionId);
    }

    public class DbService : IDbService
    {
        private readonly InsideTestDbContext _insideTestDbContext;

        public DbService(InsideTestDbContext insideDbContext)
        {
            _insideTestDbContext = insideDbContext;
        }

        public async Task AddMessage(Message message)
        {
            await _insideTestDbContext.Messages.AddAsync(message);
            await _insideTestDbContext.SaveChangesAsync();
        }

        public async Task<int> GetMessageCount(long sessionId)
        {
            return await _insideTestDbContext.Messages.CountAsync(x => x.SessionId == sessionId);
        }
    }


}
