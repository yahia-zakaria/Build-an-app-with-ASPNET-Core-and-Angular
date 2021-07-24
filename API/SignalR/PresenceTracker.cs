using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PresenceTracker
    {
        Dictionary<string, List<string>> onlineUsers = new Dictionary<string, List<string>>();
        public Task<bool> UserConnected(string username, string connectionId)
        {
            bool isOnline = false;
            lock (onlineUsers)
            {
                if (onlineUsers.ContainsKey(username))
                {
                    onlineUsers[username].Add(connectionId);
                }
                else
                {
                    onlineUsers.Add(username, new List<string>() { connectionId });
                    isOnline = true;
                }
            }
                return Task.FromResult(isOnline);
        }
        public Task<bool> UserDisconnected(string username, string connectionId)
        {
            bool isOffline = false;
            lock (onlineUsers)
            {
                if (!onlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);

                onlineUsers[username].Remove(connectionId);

                if (onlineUsers[username].Count == 0) {
                    onlineUsers.Remove(username);
                    isOffline = true;
                }
            }

             return Task.FromResult(isOffline);
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] users;
            lock (onlineUsers)
            {
                users = onlineUsers.OrderBy(o => o.Key).Select(s => s.Key).ToArray();
            }
            return Task.FromResult(users);
        }

        public Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connectionIds;
            lock (onlineUsers)
            {
                connectionIds = onlineUsers.GetValueOrDefault(username);
            }
            return Task.FromResult(connectionIds);
        }
    }
}