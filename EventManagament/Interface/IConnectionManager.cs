using System.Collections.Concurrent;

namespace EventManagament.Interface
{
    public interface IConnectionManager
    {
        void AddConnection(string userId, string connectionId);
        void RemoveConnection(string userId);
        string GetConnectionId(string userId);
        bool UserExists(string userId);
    }
    public class ConnectionManager : IConnectionManager
    {
        private static ConcurrentDictionary<string, string> _userConnections = new ConcurrentDictionary<string, string>();

        // Thêm ConnectionId cho userId
        public void AddConnection(string userId, string connectionId)
        {
            _userConnections[userId] = connectionId;
        }

        // Xóa ConnectionId khi user ngắt kết nối
        public void RemoveConnection(string userId)
        {
            _userConnections.TryRemove(userId, out _);
        }

        // Lấy ConnectionId của user
        public string GetConnectionId(string userId)
        {
            _userConnections.TryGetValue(userId, out var connectionId);
            return connectionId;
        }

        // Kiểm tra xem user có kết nối hay không
        public bool UserExists(string userId)
        {
            return _userConnections.ContainsKey(userId);
        }
    }

}
