using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Connector;
using System;

namespace AgentTransferBot
{
    [Serializable]
    public class Agent
    {
        public Agent()
        {

        }

        public Agent(Activity activity)
        {
            ConversationReference = activity.ToConversationReference();
        }

        public ConversationReference ConversationReference { get; set; }
    }
}