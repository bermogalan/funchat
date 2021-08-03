using Kritner.OrleansGettingStarted.GrainInterfaces;
using Orleans.TestKit;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Kritner.OrleansGettingStarted.Grains.Test
{
    public class ChatRoomTest : TestKitBase
    {        

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
    }
}
