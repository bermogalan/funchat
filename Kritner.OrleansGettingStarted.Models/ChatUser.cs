using System;
using System.Collections.Generic;
using System.Text;

namespace Kritner.OrleansGettingStarted.Models
{
    public class ChatUser
    {
        public string UserName { get; set; }

        public string RoomId { get; set; }

        public DateTime TimeStamp { get; set; }

        public DateTime? LastMessageRead { get; set; }
    }
}
