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

        public Agent(IActivity activity)
        {
            ConversationReference = activity.ToConversationReference();
            AgentId = activity.From.Id;
        }

        public string AgentId { get; set; }
        public ConversationReference ConversationReference { get; set; }
    }
}