using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Ms1.Api.Services;
using Ms1.Api.Store.Services;
using System.Threading.Tasks;

namespace Ms1.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IDbService _dbService;
        private readonly IWsService _wsService;

        public MessageController(IDbService dbService, IWsService wsServcie)
        {
            _dbService = dbService;
            _wsService = wsServcie;
        }

        [HttpGet("/start")]
        public async Task Start()
        {
            await _wsService.SendMessage();

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
