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
        Task<AgentMetaData> GetAgentMetadataAsync(IAddress agentAddress, CancellationToken cancellationToken);
        Task<Agent> GetAgentFromUserStateAsync(IAddress userAddress, CancellationToken cancellationToken);
        Task<User> GetUserFromAgentStateAsync(IAddress agentAddress, CancellationToken cancellationToken);
        Task StopAgentUserConversationAsync(IAddress userAddress, IAddress agentAddress, CancellationToken cancellationToken);
    }
}