using EventManagament.Interface;
using EventManagament.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Data.SqlClient;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class NotificationProcessor : BackgroundService
{
    private readonly string _connectionString;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationProcessor(string connectionString, IHubContext<NotificationHub> hubContext)
    {
        _connectionString = connectionString;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(
                    @"SELECT TOP 1 Id, Message, SentTo 
                      FROM NotificationQueue 
                      WHERE Processed = 0 
                      ORDER BY CreatedAt", connection);

                var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    int id = reader.GetInt32(0);
                    string message = reader.GetString(1);
                    string sentTo = reader.GetString(2);

                    // Gửi thông báo qua SignalR
                    var userIds = sentTo.Split(','); // Chia userIds nếu là chuỗi

                    foreach (var userId in userIds)
                    {
                        await _hubContext.Clients.User(userId).SendAsync("ReceiveMessage", message);
                    }

                    // Đánh dấu thông báo đã xử lý
                    reader.Close();
                    var updateCommand = new SqlCommand(
                        "UPDATE NotificationQueue SET Processed = 1 WHERE Id = @Id", connection);
                    updateCommand.Parameters.AddWithValue("@Id", id);
                    await updateCommand.ExecuteNonQueryAsync();
                }

                await Task.Delay(1000, stoppingToken); // Tạm dừng để tránh quá tải
            }
        }
    }
}
