using Common.Models;
using Gis.WsServer.Common;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebSocketManager;

[ApiController]
public class WebSocketController : Controller
{

    private WebSocketHandler webSocketHandler { get; set; }

    public WebSocketController(WebSocketHandler messageHandler)
    {
        webSocketHandler = messageHandler;
    }

    [HttpPost]
    [Route("/SendMessage/{MsName}")]
    public async Task SendMessage([FromRoute] string MsName , [FromBody] Message message)
    {
        await webSocketHandler.SendMessageAsync(MsName, message);
    }

}
