using Kritner.OrleansGettingStarted.GrainInterfaces;
using Kritner.OrleansGettingStarted.Grains.GrainLogic;
using Kritner.OrleansGettingStarted.Models;
using Moq;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kritner.OrleansGettingStarted.Grains.Tests.CulsterClass
{
    public class Fixture
    {
        public IGrainFactory GrainFactory;
        public IChatRoom Accumulator;
        public ChatRoomState GrainState;
        public int StateWriteCount = 0;

        public ChatRoomLogic Sut;

        public Fixture()
        {
            Accumulator = Mock.Of<IChatRoom>();
            GrainFactory = Mock.Of<IGrainFactory>(
                gf => gf.GetGrain<IChatRoom>("WORDCOUNT", null) == Accumulator);
            GrainState = new ChatRoomState();

            Sut = new ChatRoomLogic(
                state: GrainState,
                grainFactory: GrainFactory,
                writeState: IncrementWriteCount
                );
        }

        private Task IncrementWriteCount()
        {
            StateWriteCount++;
            return Task.CompletedTask;
        }

        public void VerifyEnterRoomCalledWithParams(string userName, string chatRoomId, string password)
        {
            Mock.Get(Accumulator).Verify(a => a.EnterRoom(userName, chatRoomId, password));
        }
    }
}
