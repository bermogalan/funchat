using System;
using System.Collections.Generic;
using System.Text;

namespace Kritner.OrleansGettingStarted.Models
{
    public class ChatMessage
    {
        public string UserName { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Message { get; set; }
    }
}
