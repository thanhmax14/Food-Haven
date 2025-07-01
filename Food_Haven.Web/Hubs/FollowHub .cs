using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Food_Haven.Web.Hubs
{
    public class FollowHub : Hub
    {
        public async Task SendFollowUpdate(string userId, Guid storeId, bool isFollowing)
        {
            await Clients.All.SendAsync("ReceiveFollowUpdate", storeId, isFollowing);
        }
    }
}
