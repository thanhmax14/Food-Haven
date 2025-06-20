using BusinessLogic.Services.Message;
using BusinessLogic.Services.MessageImages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Models;

namespace Food_Haven.Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly UserManager<AppUser> _userManager;
        private static readonly Dictionary<string, string> _userConnections = new();
        private static readonly Dictionary<string, Dictionary<string, DateTime>> _typingUsers = new();
        private readonly IMessageImageService _messageImageService;
        private readonly IMessageService _messageService;

        public ChatHub(UserManager<AppUser> userManager, IMessageImageService messageImageService, IMessageService messageService)
        {
            _userManager = userManager;
            _messageImageService = messageImageService;
            _messageService = messageService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections[userId] = Context.ConnectionId;
                await Clients.Others.SendAsync("UserOnline", userId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections.Remove(userId);

                if (_typingUsers.ContainsKey(userId))
                {
                    var typingWith = _typingUsers[userId].Keys.ToList();
                    foreach (var otherUserId in typingWith)
                    {
                        if (_userConnections.ContainsKey(otherUserId))
                        {
                            await Clients.Client(_userConnections[otherUserId])
                                .SendAsync("UserStoppedTyping", userId);
                        }
                    }
                    _typingUsers.Remove(userId);
                }

                await Clients.Others.SendAsync("UserOffline", userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string toUserId, string messageText, string messageId, string repliedToId = null)
        {
            var fromUserId = Context.UserIdentifier;
            var fromUser = await _userManager.FindByIdAsync(fromUserId);

            var newMessage = new Message
            {
                ID = Guid.Parse(messageId),
                FromUserId = fromUserId,
                ToUserId = toUserId,
                MessageText = messageText,
                SentAt = DateTime.Now,
                IsRead = false,
                HasDropDown = true,
                RepliedToMessageId = !string.IsNullOrEmpty(repliedToId) ? Guid.Parse(repliedToId) : null
            };

            await _messageService.AddAsync(newMessage);
            await _messageService.SaveChangesAsync();

            var messageData = new
            {
                id = newMessage.ID,
                from_id = newMessage.FromUserId,
                to_id = newMessage.ToUserId,
                msg = newMessage.MessageText,
                datetime = newMessage.SentAt.ToString("hh:mm tt"),
                senderName = fromUser?.UserName ?? "Unknown",
                senderAvatar = fromUser?.ImageUrl,
                isReplied = newMessage.RepliedToMessageId,
                is_read = false
            };

            if (_userConnections.ContainsKey(toUserId))
            {
                await Clients.Client(_userConnections[toUserId]).SendAsync("ReceiveMessage", messageData);
            }

            await Clients.Caller.SendAsync("MessageSent", messageData);
        }

        public async Task StartTyping(string toUserId)
        {
            var fromUserId = Context.UserIdentifier;

            if (!_typingUsers.ContainsKey(fromUserId))
            {
                _typingUsers[fromUserId] = new Dictionary<string, DateTime>();
            }

            _typingUsers[fromUserId][toUserId] = DateTime.Now;

            if (_userConnections.ContainsKey(toUserId))
            {
                var fromUser = await _userManager.FindByIdAsync(fromUserId);
                await Clients.Client(_userConnections[toUserId])
                    .SendAsync("UserStartedTyping", fromUserId, fromUser?.UserName ?? "Someone");
            }
        }

        public async Task StopTyping(string toUserId)
        {
            var fromUserId = Context.UserIdentifier;

            if (_typingUsers.ContainsKey(fromUserId))
            {
                _typingUsers[fromUserId].Remove(toUserId);

                if (!_typingUsers[fromUserId].Any())
                {
                    _typingUsers.Remove(fromUserId);
                }
            }

            if (_userConnections.ContainsKey(toUserId))
            {
                await Clients.Client(_userConnections[toUserId])
                    .SendAsync("UserStoppedTyping", fromUserId);
            }
        }

        public async Task MarkAsRead(string messageId, string fromUserId)
        {
            var toUserId = Context.UserIdentifier;

            if (_userConnections.ContainsKey(fromUserId))
            {
                await Clients.Client(_userConnections[fromUserId])
                    .SendAsync("MessagesRead", messageId, toUserId);
            }

            // ✅ Cập nhật trạng thái đã đọc trong DB
            var message = await _messageService.GetAsyncById(Guid.Parse(messageId));
            if (message != null && !message.IsRead)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.Now;
                await _messageService.UpdateAsync(message);
            }
        }

        public static void CleanupTypingUsers()
        {
            var cutoff = DateTime.Now.AddSeconds(-10);
            var toRemove = new List<string>();

            foreach (var user in _typingUsers)
            {
                var expiredChats = user.Value.Where(kv => kv.Value < cutoff).Select(kv => kv.Key).ToList();
                foreach (var chatId in expiredChats)
                {
                    user.Value.Remove(chatId);
                }

                if (!user.Value.Any())
                {
                    toRemove.Add(user.Key);
                }
            }

            foreach (var userId in toRemove)
            {
                _typingUsers.Remove(userId);
            }
        }
    }

    public class TypingCleanupService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                ChatHub.CleanupTypingUsers();
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
