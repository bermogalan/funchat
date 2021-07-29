﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kritner.OrleansGettingStarted.Client.Helpers;
using Kritner.OrleansGettingStarted.GrainInterfaces;
using Orleans;

namespace Kritner.OrleansGettingStarted.Client.OrleansFunctionExamples
{
    public class EverythingIsOkReminder : IOrleansFunction
    {
        public string Description => "Demonstrates a reminder service, notifying the user that everything is ok... every three seconds...";

        public async Task PerformFunction(IClusterClient clusterClient, string userName)
        {
            var grain = clusterClient.GetGrain<IEverythingIsOkGrain>(
                $"{nameof(IEverythingIsOkGrain)}-{Guid.NewGuid()}"
            );

            Console.WriteLine("Starting everything's ok alerm after key press.");
            Console.ReadKey();

            Console.WriteLine("Starting everything's ok reminder...");
            await grain.Start();

            Console.WriteLine("Reminder started.  Press any key to stop reminder.");
            Console.ReadKey();

            await grain.Stop();            

            ConsoleHelpers.ReturnToMenu();
        }
    }
}
