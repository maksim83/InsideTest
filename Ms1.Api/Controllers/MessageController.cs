using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Ms1.Api.Store.Services;
using System.Threading.Tasks;

namespace Ms1.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IDbService _dbService;

        public MessageController(IDbService dbService)
        {
            _dbService = dbService;
        }

         [HttpGet("/start")]
         public async Task Start()
         {
             await Task.CompletedTask;

         }

        [HttpGet("/stop")]
        public async Task Stop()
        {
            await Task.CompletedTask;
        }




        [HttpPost]
        public async Task<Message> CommitMessage([FromBody] Message message)
        {
            return await _dbService.AddMessage(message);
        }
    }
}
