using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MonitoringAndCommunication.Services;
using System.Runtime.CompilerServices;

namespace MonitoringAndCommunication.Controllers
  
{
    [ApiController]
    [Route("[controller]")]
    public class SimulationController: ControllerBase
    {
        private readonly IHubContext<SignalRHub> _hubContext;

        public SimulationController(IHubContext<SignalRHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage(string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
            return Ok();
        }
    }
}
