using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;

namespace AgentTransferBot
{
    public interface IAgentService
    {
        Task<bool> IsInExistingConversation(IActivity activity);
        Task<bool> RegisterAgent(IActivity activity);
        Task<bool> UnregisterAgent(IActivity activity);
        Task<AgentMetaData> GetAgentMetadata(IAddress agentAddress);
        Task<Agent> GetAgentFromUserState(IAddress userAddress);
        Task<User> GetUserFromAgentState(IAddress agentAddress);
        Task StopAgentUserConversation(IAddress userAddress, IAddress agentAddress);
    }
}