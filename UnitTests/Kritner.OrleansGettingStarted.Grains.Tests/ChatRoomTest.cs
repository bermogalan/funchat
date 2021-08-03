using Kritner.OrleansGettingStarted.GrainInterfaces;
using Orleans.TestKit;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Kritner.OrleansGettingStarted.Grains.Test
{
    public class ChatRoomTest : TestKitBase
    {
        [Fact]
        public async Task ChatRoomSendMessageTest()
        {
            IChatRoom grain = await Silo.CreateGrainAsync<ChatRoom>("id");

            // get a brand new grain to test
            var userName = "test";
            var chatRoomId = "id";
            var chatRoomPassword = "pass";
            var message = "hello!";

            var roomCreated = await grain.CreateRoom(userName, chatRoomId, chatRoomPassword);
            var enteredRoom = await grain.EnterRoom(userName, chatRoomId, chatRoomPassword);
            var sentSuccessfully = await grain.SendMessage(userName, chatRoomId, message);

            var messages = await grain.GetMessages(10, chatRoomId);
            var foundMessage = messages.FirstOrDefault(i => i.Contains(userName) && i.Contains(message));

            Assert.NotNull(foundMessage);
        }

        [Fact]
        public async Task ChatRoomCreateRoomTest() 
        {
            IChatRoom grain = await Silo.CreateGrainAsync<ChatRoom>("id");

            // get a brand new grain to test
            var userName = "test";
            var chatRoomId = "id";
            var chatRoomPassword = "pass";

            var result = await grain.CreateRoom(userName, chatRoomId, chatRoomPassword);

            Assert.True(result);
        }

        [Fact]
        public async Task ChatRoomEnterRoomNotExistingTest()
        {
            IChatRoom grain = await Silo.CreateGrainAsync<ChatRoom>("id");

            // get a brand new grain to test
            var userName = "test";
            var chatRoomId = "id";
            var chatRoomPassword = "pass";

            var result = await grain.EnterRoom(userName, chatRoomId, chatRoomPassword);

            Assert.False(result);
        }

        [Fact]
        public async Task ChatRoomEnterRoomExistingTest()
        {
            IChatRoom grain = await Silo.CreateGrainAsync<ChatRoom>("id");

            // get a brand new grain to test
            var userName = "test";
            var chatRoomId = "id";
            var chatRoomPassword = "pass";

            var roomCreated = await grain.CreateRoom(userName, chatRoomId, chatRoomPassword);
            var result = await grain.EnterRoom(userName, chatRoomId, chatRoomPassword);

            Assert.True(result);
        }

        [Fact]
        public async Task ChatRoomEnterRoomPasswordIncorrectTest()
        {
            IChatRoom grain = await Silo.CreateGrainAsync<ChatRoom>("id");

            // get a brand new grain to test
            var userName = "test";
            var chatRoomId = "id";
            var chatRoomPassword = "pass";
            var chatRoomIncorrectPassword = "pass1";

            var roomCreated = await grain.CreateRoom(userName, chatRoomId, chatRoomPassword);
            var result = await grain.EnterRoom(userName, chatRoomId, chatRoomIncorrectPassword);

            Assert.False(result);
        }

        [Fact]
        public async Task ChatRoomDeleteRoomTest()
        {
            IChatRoom grain = await Silo.CreateGrainAsync<ChatRoom>("id");

            // get a brand new grain to test
            var userName = "test";
            var chatRoomId = "id";
            var chatRoomPassword = "pass";

            var roomCreated = await grain.CreateRoom(userName, chatRoomId, chatRoomPassword);
            var rooms = await grain.GetChatRooms(userName, false);
            await grain.DeleteRoom(chatRoomId);
            var roomsAfterDelete = await grain.GetChatRooms(userName, false);

            Assert.Null(roomsAfterDelete.FirstOrDefault(r => r.RoomId == chatRoomId));
        }

        [Fact]
        public async Task ChatRoomLeaveRoomTest()
        {
            IChatRoom grain = await Silo.CreateGrainAsync<ChatRoom>("id");

            // get a brand new grain to test
            var userName = "test";
            var chatRoomId = "id";
            var chatRoomPassword = "pass";

            var roomCreated = await grain.CreateRoom(userName, chatRoomId, chatRoomPassword);
            var enteredRoom = await grain.EnterRoom(userName, chatRoomId, chatRoomPassword);
            await grain.LeaveRoom(userName, chatRoomId);

            var users = await grain.GetUsers(chatRoomId);
            var userNotFound = users.FirstOrDefault(i => i == userName);

            Assert.Null(userNotFound);
        }

        [Fact]
        public async Task ChatRoomGetMessagesTest()
        {
            IChatRoom grain = await Silo.CreateGrainAsync<ChatRoom>("id");

            // get a brand new grain to test
            var userName = "test";
            var chatRoomId = "id";
            var chatRoomPassword = "pass";
            var dateTimeStarted = DateTime.Now;
            var message = "hello!";

            var roomCreated = await grain.CreateRoom(userName, chatRoomId, chatRoomPassword);
            var enteredRoom = await grain.EnterRoom(userName, chatRoomId, chatRoomPassword);
            var sentSuccessfully1 = await grain.SendMessage(userName, chatRoomId, message);
            var sentSuccessfully2 = await grain.SendMessage(userName, chatRoomId, message);
            var sentSuccessfully3 = await grain.SendMessage(userName, chatRoomId, message);

            var messages = await grain.ReadMessagesFromDate(userName, chatRoomId, dateTimeStarted);
            
            Assert.True(messages.Count == 4);
        }
    }
}
