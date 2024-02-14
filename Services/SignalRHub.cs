using Microsoft.AspNetCore.SignalR;

namespace MonitoringAndCommunication.Services
{

    public class SignalRHub : Hub
    {
        private static Dictionary<string, string> ConnectedClients = new Dictionary<string, string>();
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var userId = Context.GetHttpContext().Request.Query["userId"];
            ConnectedClients[userId] = connectionId;
            Console.WriteLine("am ajuns aici");
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task SendMessageToUser(string message, int userId)
        {
            var connectionId = "";
            var canGetVal = ConnectedClients.TryGetValue(userId.ToString(), out connectionId);
            if (canGetVal)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
            }
            
        }
    }

}
