using Microsoft.AspNetCore.SignalR;

namespace Food_Haven.Web.Hubs
{
    public class CartHub : Hub
    {
        public async Task SendCartUpdate(string userId)
        {
            await Clients.User(userId).SendAsync("ReceiveCartUpdate");
        }
        public async Task NotifyVariantChange(string productTypeId, decimal newPrice, int newStock)
        {
            // Chuyển price sang double cho client dễ xử lý JS
            await Clients.All.SendAsync("VariantUpdated", productTypeId, (double)newPrice, newStock);
        }

    }
}
