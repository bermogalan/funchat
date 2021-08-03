using Kritner.OrleansGettingStarted.GrainInterfaces;
using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Kritner.OrleansGettingStarted.Models;
using Kritner.OrleansGettingStarted.Grains.GrainLogic;

namespace Kritner.OrleansGettingStarted.Grains
{
    [StorageProvider(ProviderName = Constants.OrleansMemoryProvider)]
    public class ChatRoom : Grain<ChatRoomState>, IChatRoom, IGrainMarker
    {
        private ChatRoomLogic _logic;

        public override Task OnActivateAsync()
        {
            _logic = new ChatRoomLogic(
                    state: State,
                    grainFactory: GrainFactory,
                    writeState: WriteStateAsync
                    );

            return base.OnActivateAsync();
        }

        public Task<bool> EnterRoom(string userName, string chatRoomId, string password) => _logic.EnterRoom(userName, chatRoomId, password);

        public Task<bool> CreateRoom(string userName, string chatRoomId, string password) => _logic.CreateRoom(userName, chatRoomId, password);
        public Task LeaveRoom(string userName, string chatRoomId) => _logic.LeaveRoom(userName, chatRoomId);

        public Task DeleteRoom(string chatRoomId) => _logic.DeleteRoom(chatRoomId);

        public Task<List<string>> GetMessages(int count, string chatRoomId) => _logic.GetMessages(count, chatRoomId);

        public Task<bool> SendMessage(string userName, string chatRoomId, string message) => _logic.SendMessage(userName, chatRoomId, message);

        public Task<List<string>> ReadMessages(string userName, string chatRoomId) => _logic.ReadMessages(userName, chatRoomId);

        public Task<List<string>> ReadMessagesFromDate(string userName, string chatRoomId, DateTime dateTimeSince) => _logic.ReadMessagesFromDate(userName, chatRoomId, dateTimeSince);

        public Task<List<string>> GetUsers(string chatRoomId) => _logic.GetUsers(chatRoomId);

        public Task<List<Models.ChatRoom>> GetChatRooms(string userName, bool isAdmin) => _logic.GetChatRooms(userName, isAdmin);

        public Task<List<Models.ChatRoom>> GetChatRoomsJoined(string userName, bool isAdmin) => _logic.GetChatRoomsJoined(userName, isAdmin);
    }    
}
