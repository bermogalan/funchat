using Kritner.OrleansGettingStarted.GrainInterfaces;
using Kritner.OrleansGettingStarted.Grains.Tests.CulsterClass;
using Kritner.OrleansGettingStarted.Models;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kritner.OrleansGettingStarted.Grains.Test
{
    [Collection(nameof(ClusterCollection))]
    public class ChatRoomTest
    {
        private readonly Fixture fixture;

        public ChatRoomTest(Fixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task Saves_State()
        {
            // get a brand new grain to test
            var grain = new Fixture();
            var userName = "test";
            var chatRoomId = "id";
            var chatRoomPassword = "pass";

            // set its value to something we can check
            await grain.Sut.EnterRoom(userName, chatRoomId, chatRoomPassword);

            // assert that state was saved by one of the silos
            grain.VerifyEnterRoomCalledWithParams(userName, chatRoomId, chatRoomPassword);
            
            // assert that state is of the corect type
            //var obj = state.State as ChatRoomState;
            //Assert.NotNull(obj);

            // assert that state has the correct value
            //Assert.NotNull(obj.ChatRooms);
        }
    }
}
