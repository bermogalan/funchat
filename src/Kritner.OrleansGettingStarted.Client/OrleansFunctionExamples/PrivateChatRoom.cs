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
    public class PrivateChatRoom : IOrleansFunction
    {
        private bool _shouldBreakLoop;
        private DateTime _lastMessageRead = DateTime.Now;
        private bool _isAdmin = false;
        public string Description => "Chat Room";

        private void DisplayOptions()
        {
            ConsoleHelpers.LineSeparator();
            Console.WriteLine("Option A: Enter General Channel");
            Console.WriteLine("Option B: Create Private Channel");
            Console.WriteLine("Option C: List Channels available");
            Console.WriteLine("Option D: Enter Channel (To view recent messages type SHOW HISTORY)");
            Console.WriteLine("          *To view all users type SHOW USERS");
            Console.WriteLine("          *To view recent messages type SHOW HISTORY");
            Console.WriteLine("          *To exit chat room type EXIT");
            Console.WriteLine("Option E: Go back to Main Menu");

            if (_isAdmin) 
            {
                Console.WriteLine("Option F: View Channel Users");
                Console.WriteLine("Option G: Delete Channel");
            }

            ConsoleHelpers.DisplayOptionsAskWhatToDo();
        }

        public async Task PerformFunction(IClusterClient clusterClient, string userName)
        {
            _isAdmin = userName == "Admin";

            DisplayOptions();

            var grain = clusterClient.GetGrain<IChatRoom>("FunChat");
            var roomExists = await grain.CreateRoom("Admin", "General", "");
            var enteredGeneralRoom = await grain.EnterRoom(userName, "General", "");

            var action = Console.ReadKey();

            while (action.Key != ConsoleKey.E)
            {
                if (action.Key == ConsoleKey.A)
                {
                    ConsoleHelpers.LineSeparator();

                    var chatRoomName = "General";

                    CancellationTokenSource tokenSource = new CancellationTokenSource();
                    CancellationToken token = tokenSource.Token;
                    Task.Run(() => GetMessages(clusterClient, chatRoomName, userName, token));

                    var chatMessage = "";

                    do
                    {
                        Console.Write("\n");
                        chatMessage = Console.ReadLine();

                        if (chatMessage == "SHOW USERS")
                        {
                            var users = await grain.GetUsers(chatRoomName);

                            Console.WriteLine("\nCurrent Users:");
                            foreach (var user in users)
                                Console.WriteLine(user);
                        }
                        else if (chatMessage == "SHOW HISTORY")
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
                else if (action.Key == ConsoleKey.B)
                {
                    ConsoleHelpers.LineSeparator();
                    var channelPassword = ConsoleHelpers.GetChatRoomPassword();
                    var randomChannelName = ConsoleHelpers.GenerateRandomAlphaNumericString(6);

                    var roomCreated = await grain.CreateRoom(userName, randomChannelName, channelPassword);
                    if (roomCreated)
                    {
                        ConsoleHelpers.LineSeparator();
                        Console.WriteLine($"Room Created : {randomChannelName}");
                    }
                    else
                    {
                        ConsoleHelpers.LineSeparator();
                        Console.WriteLine($"Room Creation failed");
                    }

                    DisplayOptions();
                }
                else if (action.Key == ConsoleKey.C)
                {
                    var rooms = await grain.GetChatRooms(userName, _isAdmin);

                    ConsoleHelpers.LineSeparator();
                    Console.WriteLine("Chat Rooms Available:");
                    var origForeColor = Console.ForegroundColor;
                    if (rooms != null)
                    {
                        foreach (var room in rooms)
                        {
                            if (room.RoomId == "General")
                            {
                                Console.ForegroundColor = origForeColor;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                            }
                            Console.WriteLine(room.RoomId);
                        }
                        Console.ForegroundColor = origForeColor;
                    }

                    ConsoleHelpers.DisplayOptionsAskWhatToDo();
                }
                else if (action.Key == ConsoleKey.D)
                {
                    //validate max private channel 2
                    var continueLoginToChannel = true;
                    if (!_isAdmin)
                    {
                        var rooms = await grain.GetChatRoomsJoined(userName, _isAdmin);

                        if (rooms != null) 
                        {
                            if (rooms.Count >= 3) 
                            {
                                Console.WriteLine("\nReached the maximum(2) private channel connection.");
                                continueLoginToChannel = false;
                            }
                        }
                    }

                    if (continueLoginToChannel)
                    {
                        ConsoleHelpers.LineSeparator();
                        var channelName = ConsoleHelpers.GetChannelName();
                        var channelPassword = ConsoleHelpers.GetChatRoomPassword();

                        var enteredRoom = await grain.EnterRoom(userName, channelName, channelPassword);
                        if (enteredRoom)
                        {
                            ConsoleHelpers.LineSeparator();
                            Console.WriteLine($"\nWelcome {userName} to Channel : {channelName}");
                            ConsoleHelpers.LineSeparator();
                            CancellationTokenSource tokenSource = new CancellationTokenSource();
                            CancellationToken token = tokenSource.Token;
                            Task.Run(() => GetMessages(clusterClient, channelName, userName, token));

                            var chatMessage = "";

                            do
                            {
                                Console.Write("\n");
                                chatMessage = Console.ReadLine();

                                if (chatMessage == "SHOW USERS")
                                {
                                    var users = await grain.GetUsers(channelName);

                                    Console.WriteLine("\nCurrent Users:");
                                    foreach (var user in users)
                                        Console.WriteLine(user);
                                }
                                else if (chatMessage == "SHOW HISTORY")
                                {
                                    var roomMessages = await grain.GetMessages(100, channelName);

                                    foreach (var message in roomMessages)
                                        Console.WriteLine(message);
                                }
                                else if (chatMessage == "EXIT")
                                {
                                    await grain.LeaveRoom(userName, channelName);
                                    DisplayOptions();
                                }
                                else
                                {
                                    var sendMessageSuccess = await grain.SendMessage(userName, channelName, chatMessage);

                                    if (!sendMessageSuccess) 
                                    {
                                        Console.WriteLine("\nUnable to send message. Leaving room in 3secs.");
                                        await Task.Delay(TimeSpan.FromSeconds(3));
                                        break;
                                    }
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
                        else
                        {
                            Console.WriteLine("\nUnable to enter Room");
                        }
                    }

                    ConsoleHelpers.DisplayOptionsAskWhatToDo();
                }
                else if (action.Key == ConsoleKey.F && _isAdmin)
                {
                    ConsoleHelpers.LineSeparator();
                    var channelName = ConsoleHelpers.GetChannelName();

                    var users = await grain.GetUsers(channelName);

                    Console.WriteLine($"Current Users for {channelName}:");
                    foreach (var user in users)
                        Console.WriteLine(user);

                    ConsoleHelpers.DisplayOptionsAskWhatToDo();
                }
                else if (action.Key == ConsoleKey.G && _isAdmin)
                {
                    ConsoleHelpers.LineSeparator();
                    var channelName = ConsoleHelpers.GetChannelName();
                    var confirm = ConsoleHelpers.GetConfirmation($"Confirm delete of channel {channelName}? Y/N");

                    while (confirm.Key != ConsoleKey.Y && confirm.Key != ConsoleKey.N) 
                    {
                        confirm = ConsoleHelpers.GetConfirmation($"Confirm delete of channel {channelName}? Y/N");
                    }

                    if (confirm.Key == ConsoleKey.Y)
                    {
                        await grain.SendMessage(userName, channelName, "ROOM DELETED");
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        await grain.DeleteRoom(channelName);
                        Console.WriteLine($"\nRoom successfully deleted: {channelName}");
                    }

                    ConsoleHelpers.DisplayOptionsAskWhatToDo();
                }

                action = Console.ReadKey();
            }

            DisplayOptions();
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

            var continueProcess = true;
            while (continueProcess)
            {
                token.ThrowIfCancellationRequested();

                await Task.Delay(TimeSpan.FromSeconds(2));
                var grain = clusterClient.GetGrain<IChatRoom>("FunChat");
                var roomMessages = await grain.ReadMessages(userName, chatRoomName);

                foreach (var message in roomMessages)
                {
                    Console.WriteLine(message);

                    if (message.Contains("Admin") && message.Contains("ROOM DELETED"))
                    {
                        continueProcess = false;//break the loop of reading messages
                    }
                }
            }
        }
    }
}
