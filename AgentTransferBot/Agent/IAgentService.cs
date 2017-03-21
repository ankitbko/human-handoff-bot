using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;

namespace AgentTransferBot
{
    public interface IAgentService
    {
        Agent GetAgentFromUserState(IAddress userAddress);
        User GetUserFromAgentState(IAddress agentAddress);
        void StopAgentUserConversation(IAddress userAddress, IAddress agentAddress);
    }
}