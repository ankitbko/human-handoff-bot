using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgentTransferBot
{
    public interface IAgentToUser
    {
        Task SendToUserAsync(Activity message, CancellationToken cancellationToken);
    }
}
