using Common.Models;
using System.Threading.Tasks;

namespace Ms1.Api.Store.Services
{
    public interface IDbService
    {
        public Task<Message> AddMessage(Message message);
    }

    public class DbService : IDbService
    {
        private readonly InsideTestDbContext _insideTestDbContext;

        public DbService(InsideTestDbContext insideDbContext)
        {
            _insideTestDbContext = insideDbContext;

        }

        public async Task<Message> AddMessage(Message message)
        {
           var savedMessage= await _insideTestDbContext.Messages.AddAsync(message);
            await _insideTestDbContext.SaveChangesAsync();

            return savedMessage.Entity;



        }
    }


}
