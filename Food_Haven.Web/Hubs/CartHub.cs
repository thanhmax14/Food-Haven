using Microsoft.AspNetCore.SignalR;

namespace Food_Haven.Web.Hubs
{
    public class CartHub : Hub
    {
        public async Task SendCartUpdate(string userId)
        {
            await Clients.User(userId).SendAsync("ReceiveCartUpdate");
        }
       
    }
}
