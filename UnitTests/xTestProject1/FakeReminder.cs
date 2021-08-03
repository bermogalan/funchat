using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kritner.OrleansGettingStarted.Grains.Test
{
    public class FakeReminder : IGrainReminder
    {
        public FakeReminder(string reminderName, TimeSpan dueTime, TimeSpan period)
        {
            ReminderName = reminderName;
            DueTime = dueTime;
            Period = period;
        }

        public string ReminderName { get; }
        public TimeSpan DueTime { get; }
        public TimeSpan Period { get; }
    }
}
