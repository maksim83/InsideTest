using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Ms1.Api.Services;
using Ms1.Api.Store.Services;
using System;
using System.Threading.Tasks;

namespace Ms1.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IDbService _dbService;
        private readonly IWsService _wsService;
        private readonly int _processingInterval;

        private static DateTime? _startTime;
        private static bool _isCanceled;
        private static long _sessionId;

        public MessageController(IDbService dbService, IWsService wsService, IConfiguration configuration)
        {
            _dbService = dbService;
            _wsService = wsService;
            _processingInterval = configuration.GetValue<int>("ProcessingInterval");
        }

        [HttpGet("/start")]
        public async Task Start()
        {
            if (_startTime != null)
                return;

            _startTime = DateTime.Now;
            _isCanceled = false;
            _sessionId = DateTime.Now.Ticks / 10 % 1000000000;

            await _wsService.SendMessageAsync(_sessionId);
        }

        [HttpGet("/stop")]
        public void Stop()
        {
            _isCanceled = true;
            _startTime = null;
        }

        [HttpPost]
        public async Task<Message> CommitMessage([FromBody] Message message)
        {

            message.CommitMessage();
            var newMessage = await _dbService.AddMessage(message);

            if (_startTime != null && !_isCanceled && (DateTime.Now - _startTime.Value).TotalSeconds < _processingInterval)
                await _wsService.SendMessageAsync(_sessionId);

            return newMessage;
        }
    }
}
