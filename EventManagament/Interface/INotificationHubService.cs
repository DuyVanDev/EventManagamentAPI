using EventManagament.Models;
using Microsoft.AspNetCore.SignalR;

namespace EventManagament.Interface
{
    public interface INotificationHubService
    {
        Task SendNotificationAsync(string message, IEnumerable<string> userIds);
    }
    public class NotificationHubService : INotificationHubService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationHubService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync(string message, IEnumerable<string> userIds)
        {
            foreach (var userId in userIds)
            {
                await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", message);
            }
        }
    }
}
