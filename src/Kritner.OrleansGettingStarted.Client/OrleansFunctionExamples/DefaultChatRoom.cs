using Kritner.OrleansGettingStarted.Client.Helpers;
using Kritner.OrleansGettingStarted.GrainInterfaces;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Kritner.OrleansGettingStarted.Client.OrleansFunctionExamples
{
    public class DefaultChatRoom : IOrleansFunction, IObserverSample
    {
        private bool _shouldBreakLoop;
        private DateTime _lastMessageRead = DateTime.Now;
        public string Description => "Enter General Channel room";

        private void DisplayOptions()
        {
            ConsoleHelpers.LineSeparator();
            Console.WriteLine("Option A: View all users");
            Console.WriteLine("Option B: Enter chat room (To view recent messages type SHOW HISTORY)");
            Console.WriteLine("          *To view recent messages type SHOW HISTORY");
            Console.WriteLine("          *To exit chat room type EXIT");
            Console.WriteLine("Option C: Go back to Main Menu");

            ConsoleHelpers.DisplayOptionsAskWhatToDo();
        }        

        public async Task PerformFunction(IClusterClient clusterClient, string userName)
        {
            var chatRoomName = "General";

            var grain = clusterClient.GetGrain<IVisitTracker>("FunChat");
            var roomExists = await grain.CreateRoom("Admin", "General", "");

            if (roomExists) 
            {
                var enteredRoom = await grain.EnterRoom(userName, chatRoomName, "");

                if (enteredRoom)
                {
                    DisplayOptions();

                    var action = Console.ReadKey();

                    while (action.Key != ConsoleKey.C)
                    {
                        if (action.Key == ConsoleKey.A)
                        {
                            var users = await grain.GetUsers(chatRoomName);

                            Console.WriteLine("\nCurrent Users:");
                            foreach (var user in users)
                                Console.WriteLine(user);

                            ConsoleHelpers.DisplayOptionsAskWhatToDo();
                        }
                        else if (action.Key == ConsoleKey.B)
                        {
                            CancellationTokenSource tokenSource = new CancellationTokenSource();
                            CancellationToken token = tokenSource.Token;
                            Task.Run(() => GetMessages(clusterClient, chatRoomName, userName, token));

                            var chatMessage = "";

                            do
                            {
                                Console.Write("\n");
                                chatMessage = Console.ReadLine();

                                if (chatMessage == "SHOW HISTORY")
                                {
                                    var roomMessages = await grain.GetMessages(100, chatRoomName);

                                    foreach (var message in roomMessages)
                                        Console.WriteLine(message);
                                }
                                else if (chatMessage == "EXIT")
                                {
                                    DisplayOptions();
                                }
                                else
                                {
                                    await grain.SendMessage(userName, chatRoomName, chatMessage);

                                    //var roomMessages = await grain.GetMessages(10);

                                    //foreach (var message in roomMessages)
                                    //    Console.WriteLine(message);

                                    //var chatRoomSender = clusterClient.GetGrain<IObservableManager>(chatRoomName);
                                    //await chatRoomSender.SendMessageToObservers($"{DateTime.Now} {userName} : {chatMessage}");
                                }
                            }
                            while (chatMessage != "EXIT");

                            tokenSource.Cancel();
                        }

                        action = Console.ReadKey();
                    }

                    DisplayOptions();
                    await grain.LeaveRoom(userName, chatRoomName);
                }
                else
                {
                    Console.WriteLine("Unable to enter room.");
                }
            }

            
        }

        async Task GetMessages(IClusterClient clusterClient, string chatRoomName, string userName, CancellationToken token)
        {
            //////var observerManager = clusterClient.GetGrain<IObservableManager>(chatRoomName);
            //////var observerRef = await clusterClient
            //////    .CreateObjectReference<IObserverSample>(this);


            ////////while (true)
            ////////{
            ////////token.ThrowIfCancellationRequested();

            //////if (token.IsCancellationRequested)
            //////{
            //////    await observerManager.Unsubscribe(observerRef);
            //////}
            //////else
            //////{
            //////    await Task.Delay(TimeSpan.FromSeconds(1));
            //////    await observerManager.Subscribe(observerRef);
            //////}
            ////////}

            while (true)
            {
                token.ThrowIfCancellationRequested();

                await Task.Delay(TimeSpan.FromSeconds(2));
                var grain = clusterClient.GetGrain<IVisitTracker>("FunChat");
                var roomMessages = await grain.ReadMessages(userName, chatRoomName);

                foreach (var message in roomMessages)
                    Console.WriteLine(message);
            }
        }

        public void ReceiveMessage(string message)
        {
            //ConsoleHelpers.LineSeparator();
            Console.WriteLine(message);
        }
    }
}
