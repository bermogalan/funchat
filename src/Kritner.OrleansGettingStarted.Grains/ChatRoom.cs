using Kritner.OrleansGettingStarted.GrainInterfaces;
using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Kritner.OrleansGettingStarted.Models;

namespace Kritner.OrleansGettingStarted.Grains
{
    [StorageProvider(ProviderName = Constants.OrleansMemoryProvider)]
    public class ChatRoom : Grain<ChatRoomState>, IChatRoom, IGrainMarker
    {
        public async Task<bool> EnterRoom(string userName, string chatRoomId, string password)
        {
            var result = false;

            if (State.ChatRooms == null)
                State.ChatRooms = new List<Models.ChatRoom>();

            var chatRoom = State.ChatRooms.FirstOrDefault(i => i.RoomId == chatRoomId && i.Password == password);

            if (chatRoom != null)
            {
                var chatUser = chatRoom.Users.FirstOrDefault(i => i.UserName == userName);

                if (chatUser == null) 
                {
                    chatRoom.Users.Add(new ChatUser() { UserName = userName, RoomId = chatRoomId, TimeStamp = DateTime.Now });
                    AddMessage(userName, chatRoomId, $"entered the room.");

                    await WriteStateAsync();
                }

                result = true;
            }

            return result;
        }

        public async Task<bool> CreateRoom(string userName, string chatRoomId, string password) 
        {
            var result = false;

            if (State.ChatRooms == null)
                State.ChatRooms = new List<Models.ChatRoom>();

            var chatRoom = State.ChatRooms.FirstOrDefault(i => i.RoomId == chatRoomId);

            if (chatRoom == null)
            {
                chatRoom = new Models.ChatRoom() { RoomId = chatRoomId, Messages = new List<ChatMessage>(), CreatedBy = userName, Password = password, RoomName = $"Private room by {userName} Id : {chatRoomId}", Users = new List<ChatUser>() };
                //chatRoom.Users.Add(new ChatUser() { RoomId = chatRoomId, UserName = userName, TimeStamp = DateTime.Now });
                State.ChatRooms.Add(chatRoom);
                await WriteStateAsync();

                result = true;
            }
            else
                result = true;

            return result;
        }
        public async Task LeaveRoom(string userName, string chatRoomId)
        {
            if (State.ChatRooms == null)
                State.ChatRooms = new List<Models.ChatRoom>();

            var chatRoom = State.ChatRooms.FirstOrDefault(i => i.RoomId == chatRoomId);

            if (chatRoom != null)
            {
                var chatUser = chatRoom.Users.FirstOrDefault(i => i.UserName == userName);

                if (chatUser != null)
                {
                    chatRoom.Users.Remove(chatUser);

                    AddMessage(userName, chatRoomId, $"left the room.");

                    await WriteStateAsync();
                }
            }
        }

        public async Task DeleteRoom(string chatRoomId)
        {
            if (State.ChatRooms == null)
                State.ChatRooms = new List<Models.ChatRoom>();

            var chatRoom = State.ChatRooms.FirstOrDefault(i => i.RoomId == chatRoomId);

            if (chatRoom != null)
            {
                State.ChatRooms.Remove(chatRoom);
                await WriteStateAsync();
            }
        }

        public Task<List<string>> GetMessages(int count, string chatRoomId)
        {
            var result = new List<string>();

            if (State.ChatRooms == null)
                State.ChatRooms = new List<Models.ChatRoom>();

            var chatRoom = State.ChatRooms.FirstOrDefault(i => i.RoomId == chatRoomId);

            if (chatRoom != null) 
            {
                result = chatRoom.Messages.OrderByDescending(i => i.TimeStamp).Take(count).OrderBy(i => i.TimeStamp).Select(i => $"{i.TimeStamp} {i.UserName}: {i.Message}").ToList();
            }

            return Task.FromResult(result);
        }

        public async Task<bool> SendMessage(string userName, string chatRoomId, string message)
        {
            var result = AddMessage(userName, chatRoomId, message);

            await WriteStateAsync();

            return result;
        }

        public async Task<List<string>> ReadMessages(string userName, string chatRoomId)
        {
            var result = new List<string>();

            if (State.ChatRooms == null)
                State.ChatRooms = new List<Models.ChatRoom>();

            var chatRoom = State.ChatRooms.FirstOrDefault(i => i.RoomId == chatRoomId);

            if (chatRoom != null)
            {
                var chatUser = chatRoom.Users.FirstOrDefault(i => i.UserName == userName);

                if (chatUser != null)
                {
                    var lastMessageReadValue = DateTime.Now;
                    if (chatUser.LastMessageRead.HasValue == false)
                    {
                        chatUser.LastMessageRead = lastMessageReadValue;
                        await WriteStateAsync();
                    }
                    else
                    {
                        lastMessageReadValue = chatUser.LastMessageRead.Value;
                    }

                    var messagesFiltered = chatRoom.Messages.Where(i => i.TimeStamp > lastMessageReadValue).ToList();

                    if (messagesFiltered.Count > 0)
                    {
                        result = messagesFiltered.OrderBy(i => i.TimeStamp).Select(i => $"{i.TimeStamp} {i.UserName}: {i.Message}").ToList();

                        if (messagesFiltered.Max(i => i.TimeStamp) > lastMessageReadValue)
                        {
                            chatUser.LastMessageRead = messagesFiltered.Max(i => i.TimeStamp);
                            await WriteStateAsync();
                        }
                    }
                }
            }

            return result;
        }

        private bool AddMessage(string userName, string chatRoomId, string message)
        {
            var messageAdded = false;

            var chatRoom = State.ChatRooms.FirstOrDefault(i => i.RoomId == chatRoomId);

            if (chatRoom != null)
            {
                if (chatRoom.Messages == null)
                {
                    chatRoom.Messages = new List<ChatMessage>();
                }

                chatRoom.Messages.Add(new ChatMessage() { UserName = userName, Message = message, TimeStamp =DateTime.Now });

                messageAdded = true;
            }

            return messageAdded;
        }

        public Task<List<string>> GetUsers(string chatRoomId)
        {
            var result = new List<string>();
            if (State.ChatRooms == null)
                State.ChatRooms = new List<Models.ChatRoom>();

            var chatRoom = State.ChatRooms.FirstOrDefault(i => i.RoomId == chatRoomId);

            if (chatRoom != null)
            {
                result = chatRoom.Users.Select(i => $"{i.TimeStamp} {i.UserName}").ToList();
            }

            return Task.FromResult(result);
        }

        public Task<List<Models.ChatRoom>> GetChatRooms(string userName, bool isAdmin) 
        {
            var result = new List<Models.ChatRoom>();

            if (isAdmin)
            {
                result = State.ChatRooms;
            }
            else
            {
                result = State.ChatRooms.Where(i => i.CreatedBy == userName).ToList();
            }

            return Task.FromResult(result);
        }

        public Task<List<Models.ChatRoom>> GetChatRoomsJoined(string userName, bool isAdmin)
        {
            var result = new List<Models.ChatRoom>();

            if (isAdmin)
            {
                result = State.ChatRooms;
            }
            else
            {
                result = State.ChatRooms.Where(i => i.Users.FirstOrDefault(u => u.UserName == userName) != null).ToList();
            }

            return Task.FromResult(result);
        }
    }    
}
