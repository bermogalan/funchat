using Kritner.OrleansGettingStarted.Models;
using Orleans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kritner.OrleansGettingStarted.GrainInterfaces
{
    public interface IChatRoom : IGrainWithStringKey, IGrainInterfaceMarker
    {
        Task<bool> SendMessage(string userName, string chatRoomId, string message);

        Task<bool> EnterRoom(string userName, string chatRoomId, string password);

        Task<bool> CreateRoom(string userName, string chatRoomId, string password);

        Task DeleteRoom(string chatRoomId);

        Task LeaveRoom(string userName, string chatRoomId);

        Task<List<string>> GetMessages(int count, string chatRoomId);

        Task<List<string>> ReadMessages(string userName, string chatRoomId);

        Task<List<string>> ReadMessagesFromDate(string userName, string chatRoomId, DateTime dateTimeSince);

        Task<List<string>> GetUsers(string chatRoomId);

        Task<List<ChatRoom>> GetChatRooms(string userName, bool isAdmin);

        Task<List<ChatRoom>> GetChatRoomsJoined(string userName, bool isAdmin);
    }
}
