using System;
using System.Collections.Generic;

namespace Kritner.OrleansGettingStarted.Models
{
    public class ChatRoom
    {
        public string RoomId { get; set; }

        public string Password { get; set; }

        public string RoomName { get; set; }

        public string CreatedBy { get; set; }

        public List<ChatMessage> Messages { get; set; }

        public List<ChatUser> Users { get; set; }
    }
}
