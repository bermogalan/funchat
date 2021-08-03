using Kritner.OrleansGettingStarted.GrainInterfaces;
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
        private readonly ClusterFixture fixture;

        public ChatRoomTest(ClusterFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task Saves_State()
        {
            // get a brand new grain to test
            var grain = fixture.Cluster.GrainFactory.GetGrain<IChatRoom>("");

            // set its value to something we can check
            await grain.CreateRoom("", "", "");

            // assert that state was saved by one of the silos
            var state = fixture.GetChatRoome(typeof(IChatRoom), "State", grain);
            Assert.NotNull(state);

            // assert that state is of the corect type
            //var obj = state.State as ChatRoomState;
            //Assert.NotNull(obj);

            // assert that state has the correct value
            //Assert.NotNull(obj.ChatRooms);
        }
    }
}
