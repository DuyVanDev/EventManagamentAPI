using EventManagament.Interface;
using EventManagament.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace EventManagament.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

   
    public class NotificationsController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly string _connectionString;

        private readonly IConnectionManager _connectionManager;

        public NotificationsController(IHubContext<NotificationHub> hubContext, IConfiguration configuration, IConnectionManager connectionManager)
        {
            _hubContext = hubContext;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _connectionManager = connectionManager;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            // Lưu thông báo vào cơ sở dữ liệu (bảng NotificationQueue)
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand("INSERT INTO NotificationQueue (Message, SentTo) VALUES (@Message, @SentTo)", connection);
                command.Parameters.AddWithValue("@Message", request.Message);
                command.Parameters.AddWithValue("@SentTo", request.UserIds.ToString()); // Giả sử gửi cho 1 UserId cụ thể
                await command.ExecuteNonQueryAsync();

                // Gửi thông báo qua SignalR
                await _hubContext.Clients.User(request.UserIds.ToString()).SendAsync("ReceiveMessage", request.Message);
            }

            return Ok();
        }
        [HttpPost("SendToAll")]
        public async Task<IActionResult> SendNotificationToAll([FromBody] string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
            return Ok(new { message = "Notification sent to all users." });
        }

        [HttpPost("sendNotification")]
        public async Task<IActionResult> SendNotification1([FromBody] NotificationRequest request)
        {

            try
            {
                if (request.UserIds == null || !request.UserIds.Any())
                {
                    return BadRequest("Danh sách UserIds không hợp lệ.");
                }

                // Gửi thông báo qua SignalR cho từng user
                foreach (var userId in request.UserIds)
                {
                    await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveMessage", request.Message);
                    if (_connectionManager.UserExists(userId.ToString()))
                    {
                        var connectionId = _connectionManager.GetConnectionId(userId.ToString());
                        await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", request.Message);
                        return Ok();
                    }
                }

                

                // Lưu vào bảng NotificationQueue
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var command = new SqlCommand("INSERT INTO NotificationQueue (Message, SentTo, IsRead) VALUES (@Message, @SentTo, @IsRead)", connection);
                    command.Parameters.AddWithValue("@Message", request.Message);

                    // Chuyển danh sách UserIds thành JSON
                    var userIdsJson = JsonConvert.SerializeObject(request.UserIds);
                    command.Parameters.AddWithValue("@SentTo", userIdsJson);

                    command.Parameters.AddWithValue("@IsRead", 0); // Mặc định là chưa xem

                    await command.ExecuteNonQueryAsync();
                }


                return Ok("Notification added to queue and sent.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            // Thêm dữ liệu vào bảng NotificationQueue
            //using (var connection = new SqlConnection(_connectionString))
            //{
            //    await connection.OpenAsync();

            //    var command = new SqlCommand("INSERT INTO NotificationQueue (Message, SentTo) VALUES (@Message, @SentTo)", connection);
            //    command.Parameters.AddWithValue("@Message", request.Message);
            //    command.Parameters.AddWithValue("@SentTo", request.UserId.ToString()); // Gửi cho 1 UserId cụ thể
            //    command.Parameters.AddWithValue("@IsRead", 0);
            //    await command.ExecuteNonQueryAsync();
            //}

            //// Gửi thông báo qua SignalR
            //await _hubContext.Clients.User(request.UserId.ToString()).SendAsync("ReceiveMessage", request.Message);

            //if (_connectionManager.UserExists(request.UserId.ToString()))
            //{
            //    var connectionId = _connectionManager.GetConnectionId(request.UserId.ToString());
            //    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", request.Message);
            //    return Ok();
            //}

        }
    }
    //public class NotificationsController : ControllerBase
    //{
    //    private readonly IHubContext<NotificationHub> _hubContext;

    //    public NotificationsController(IHubContext<NotificationHub> hubContext)
    //    {
    //        _hubContext = hubContext;
    //    }

    //    [HttpPost("send")]
    //    public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
    //    {
    //        if (request.UserIds != null && request.UserIds.Any())
    //        {
    //            foreach (var userId in request.UserIds)
    //            {
    //                await _hubContext.Clients.Group(userId.ToString())
    //                    .SendAsync("ReceiveNotification", request.Message);
    //            }
    //            return Ok(new { Status = "Notifications sent" });
    //        }

    //        return BadRequest("No UserIds provided");
    //    }

    //}
}
