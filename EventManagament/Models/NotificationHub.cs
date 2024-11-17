using EventManagament.Interface;
using Microsoft.AspNetCore.SignalR;
namespace EventManagament.Models
{
    public class NotificationHub : Hub
    {
        private readonly IConnectionManager _connectionManager;

        public NotificationHub(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public override Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext()?.Request.Query["userId"];
            var connectionId = Context.ConnectionId;

            if (!string.IsNullOrEmpty(userId))
            {
                _connectionManager.AddConnection(userId, connectionId);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = _connectionManager.GetConnectionId(Context.ConnectionId);

            if (!string.IsNullOrEmpty(userId))
            {
                _connectionManager.RemoveConnection(userId);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToUser(string userId, string message)
        {
            if (_connectionManager.UserExists(userId))
            {
                var connectionId = _connectionManager.GetConnectionId(userId);
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
            }
        }
    }


    //public class NotificationHub : Hub
    //{
    //    // Gửi thông báo đến tất cả client kết nối
    //    public async Task SendNotification(string message)
    //    {
    //        await Clients.All.SendAsync("ReceiveMessage", message);
    //    }

    //    // Gửi thông báo tới một user cụ thể
    //    public async Task SendNotificationToUser(string userId, string message)
    //    {
    //        if (string.IsNullOrEmpty(userId))
    //        {
    //            throw new ArgumentException("UserId is required.");
    //        }
    //        Console.WriteLine($"Sending message to User: {userId}"); // Log để kiểm tra
    //        await Clients.User(userId).SendAsync("ReceiveMessage", message);
    //    }

    //    public async Task SendNotificationToAllUsers(string message)
    //    {
    //        // Gửi thông báo đến tất cả người dùng đang kết nối
    //        await Clients.All.SendAsync("ReceiveMessage", message);
    //    }
    //}
}
