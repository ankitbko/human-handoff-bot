using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System.Threading;
using System.Threading.Tasks;

namespace AgentTransferBot
{
    public interface IAgentService
    {
        Task<bool> IsInExistingConversation(IActivity activity, CancellationToken cancellationToken);
        Task<bool> RegisterAgent(IActivity activity, CancellationToken cancellationToken);
        Task<bool> UnregisterAgent(IActivity activity, CancellationToken cancellationToken);
        Task<AgentMetaData> GetAgentMetadata(IAddress agentAddress, CancellationToken cancellationToken);
        Task<Agent> GetAgentFromUserState(IAddress userAddress, CancellationToken cancellationToken);
        Task<User> GetUserFromAgentState(IAddress agentAddress, CancellationToken cancellationToken);
        Task StopAgentUserConversation(IAddress userAddress, IAddress agentAddress, CancellationToken cancellationToken);
    }
}