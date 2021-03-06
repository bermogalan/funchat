using System;
using System.Collections.Generic;
using System.Text;

namespace Kritner.OrleansGettingStarted.Client.OrleansFunctionExamples
{
    public class OrleansFunctionProvider : IOrleansFunctionProvider
    {
        public IList<IOrleansFunction> GetOrleansFunctions()
        {
            return new List<IOrleansFunction>()
            {
                //new DefaultChatRoom(),
                new PrivateChatRoom(),
                //new HelloWorld(),
                //new MultipleInstantiations(),
                //new StatefulWork(),
                //new ShowoffDashboard(),
                //new DependencyInjectionEmailService(),
                //new EverythingIsOkReminder(),
                //new GrainObserverReceiver(),
                //new GrainObserverEventSender(),
            };
        }
    }
}
