using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentTransferBot
{
    [Serializable]
    public class User
    {
        public User()
        {

        }
        public User(Activity message)
        {
            ConversationReference = message.ToConversationReference();
        }

        public ConversationReference ConversationReference { get; set; }
    }
}
