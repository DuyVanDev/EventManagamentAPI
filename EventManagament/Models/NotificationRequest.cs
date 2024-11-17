
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventManagament.Models
{
    public class NotificationRequest
    {
        public List<int> UserIds { get; set; } // Danh sách các UserId để gửi thông báo
        public string Message { get; set; }     // Nội dung thông báo
    }

}
