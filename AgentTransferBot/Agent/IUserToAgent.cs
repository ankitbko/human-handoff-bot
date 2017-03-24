using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgentTransferBot
{
    public interface IUserToAgent
    {
        Task<bool> AgentTransferRequiredAsync(Activity message, CancellationToken cancellationToken);

        Task SendToAgentAsync(Activity message, CancellationToken cancellationToken);

        Task<Agent> IntitiateConversationWithAgentAsync(Activity message, CancellationToken cancellationToken);
    }
}
