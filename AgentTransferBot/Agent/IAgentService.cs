using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System.Threading;
using System.Threading.Tasks;

namespace AgentTransferBot
{
    public interface IAgentService
    {
        Task<bool> IsInExistingConversationAsync(IActivity activity, CancellationToken cancellationToken);
        Task<bool> RegisterAgentAsync(IActivity activity, CancellationToken cancellationToken);
        Task<bool> UnregisterAgentAsync(IActivity activity, CancellationToken cancellationToken);
        Task<bool> IsAgent(IActivity activity, CancellationToken cancellationToken);
        Task<Agent> GetAgentInConversationAsync(IActivity userActivity, CancellationToken cancellationToken);
        Task<User> GetUserInConversationAsync(IActivity agentActivity, CancellationToken cancellationToken);
        Task StopAgentUserConversationAsync(IActivity userActivity, IActivity agentActivity, CancellationToken cancellationToken);
    }
}