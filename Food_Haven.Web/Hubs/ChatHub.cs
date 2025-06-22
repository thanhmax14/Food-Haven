using BusinessLogic.Services.Message;
using BusinessLogic.Services.MessageImages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Models;
using System.Collections.Concurrent;

namespace Food_Haven.Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly UserManager<AppUser> _userManager;
        private static readonly ConcurrentDictionary<string, HashSet<string>> _userConnections = new();
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
                _userConnections.AddOrUpdate(userId,
                    _ => new HashSet<string> { Context.ConnectionId },
                    (_, connections) => { connections.Add(Context.ConnectionId); return connections; });

                await Clients.Others.SendAsync("UserOnline", userId);
            }

            await base.OnConnectedAsync();
        }
        public static bool IsUserOnline(string userId)
        {
            return _userConnections.ContainsKey(userId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                if (_userConnections.TryGetValue(userId, out var connections))
                {
                    connections.Remove(Context.ConnectionId);
                    if (connections.Count == 0)
                    {
                        _userConnections.TryRemove(userId, out _);

                        if (_typingUsers.ContainsKey(userId))
                        {
                            var typingWith = _typingUsers[userId].Keys.ToList();
                            foreach (var otherUserId in typingWith)
                            {
                                if (_userConnections.TryGetValue(otherUserId, out var toConnections))
                                {
                                    foreach (var conn in toConnections)
                                    {
                                        await Clients.Client(conn).SendAsync("UserStoppedTyping", userId);
                                    }
                                }
                            }
                            _typingUsers.Remove(userId);
                        }

                       


                        var user = await _userManager.FindByIdAsync(userId);
                        if (user != null)
                        {
                            user.LastAccess = DateTime.Now;
                            await _userManager.UpdateAsync(user);
                        }
                        await Clients.Others.SendAsync("UserOffline", userId, user.LastAccess.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }
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

            if (_userConnections.TryGetValue(toUserId, out var toConnections))
            {
                foreach (var conn in toConnections)
                {
                    await Clients.Client(conn).SendAsync("ReceiveMessage", messageData);
                }
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

            if (_userConnections.TryGetValue(toUserId, out var toConnections))
            {
                var fromUser = await _userManager.FindByIdAsync(fromUserId);
                foreach (var conn in toConnections)
                {
                    await Clients.Client(conn).SendAsync("UserStartedTyping", fromUserId, fromUser?.UserName ?? "Someone");
                }
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

            if (_userConnections.TryGetValue(toUserId, out var toConnections))
            {
                foreach (var conn in toConnections)
                {
                    await Clients.Client(conn).SendAsync("UserStoppedTyping", fromUserId);
                }
            }
        }

        public async Task MarkAsRead(string messageId, string fromUserId)
        {
            var toUserId = Context.UserIdentifier;

            if (_userConnections.TryGetValue(fromUserId, out var fromConnections))
            {
                foreach (var conn in fromConnections)
                {
                    await Clients.Client(conn).SendAsync("MessagesRead", messageId, toUserId);
                }
            }

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
