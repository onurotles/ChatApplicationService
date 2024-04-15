using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApplication.DataService;
using ChatApplication.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatApplication.Hubs
{
    public class ChatHub : Hub
    {
        private readonly SharedDb _sharedDb;
        public ChatHub(SharedDb sharedDb) => _sharedDb = sharedDb;
        public async Task JoinChat(UserConnection connection) {
            await Clients.All.SendAsync("ReceiveMessage", "admin", $"{connection.UserName} has joined");
        }

        public async Task JoinSpecificChatRoom(UserConnection connection) {
            await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);
            _sharedDb.connections[Context.ConnectionId] = connection;
            await Clients.Group(connection.ChatRoom).SendAsync("JoinSpecificChatRoom", "admin", $"{connection.UserName} has joined {connection.ChatRoom}");
        }

        public async Task SendMessage(string msg) {
            if (_sharedDb.connections.TryGetValue(Context.ConnectionId, out UserConnection conn)) {
                await Clients.Group(conn.ChatRoom).SendAsync("ReceiveSpecificMessage", conn.UserName, msg);
            }
        }
    }
}