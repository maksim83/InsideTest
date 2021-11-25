using Common.Infrastructure.Utils;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Ms1.Api.Services;
using Ms1.Api.Store.Services;
using OpenTracing;
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
        private readonly ITracer _tracer;

        private static DateTime? _startTime;
        private static bool _isCanceled;
        private static long _sessionId;

        public MessageController(IDbService dbService, IWsService wsService, IConfiguration configuration, ITracer tracer)
        {
            _dbService = dbService;
            _wsService = wsService;
            _processingInterval = configuration.GetValue<int>("ProcessingInterval");
            _tracer = tracer;
        }

        [HttpGet("/start")]
        public async Task Start()
        {
            if (_startTime != null)
                return;

            _startTime = DateTime.Now;
            _isCanceled = false;
            _sessionId = DateTime.Now.Ticks / 10 % 1000000000;


            while (_startTime != null && !_isCanceled && (DateTime.Now - _startTime.Value).TotalSeconds < _processingInterval)
                await _wsService.SendMessageAsync(_sessionId);

            Console.WriteLine($"Messages processed:{ await _dbService.GetMessageCount(_sessionId)}\nTime elapsed: {DateTime.Now - (_startTime != null ? _startTime.Value : DateTime.Now)}");
        }

        [HttpGet("/stop")]
        public void Stop()
        {
            _isCanceled = true;
        }

        [HttpPost]
        public async Task CommitMessage([FromBody] Message message)
        {

            await JaegerUtils.SetSpan(_tracer, message, "Recieve from Ms3");

            message.CommitMessage();
            await _dbService.AddMessage(message);



        }
    }
}
