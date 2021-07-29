using System;
using System.Threading.Tasks;
using Kritner.OrleansGettingStarted.GrainInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orleans;
using Orleans.TestingHost;
using Xunit;

namespace Kritner.OrleansGettingStarted.Grains.Tests
{
    [TestClass]
    public class ChatRoomTest
    {
        [Fact]
        public async Task SaysHelloCorrectly()
        {
            var builder = new TestClusterBuilder();
            builder.Options.ServiceId = Guid.NewGuid().ToString();
            var cluster = builder.Build();
            cluster.Deploy();

            var userName = "bermo";
            var chatRoomId = "RoomA";
            var chatRoomPassword = "PassA";

            //var grain = cluster.GrainFactory.GetGrain<IChatRoom>("Test");
            //grain.CreateRoom(userName, chatRoomId, chatRoomPassword);

            //cluster.StopAllSilos();

            //Assert.Equals("Hello, World", greeting);
        }
    }
}
