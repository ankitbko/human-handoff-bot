using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentTransferBot
{
    public interface IUserToAgent
    {
        bool AgentTransferRequired(Activity message);

        Task SendToAgent(Activity message);

        Task<Agent> IntitiateConversationWithAgent(Activity message);
    }
}
