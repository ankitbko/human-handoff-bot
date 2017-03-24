using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static AgentTransferBot.Utilities;

namespace AgentTransferBot
{
    public class AgentService : IAgentService
    {
        private const string AGENT_METADATA_KEY = "AgentMetaData";
        private readonly IAgentProvider _agentProvider;
        private readonly IBotDataStore<BotData> _botDataStore;

        public AgentService(IAgentProvider agentProvider, IBotDataStore<BotData> botDataStore, IActivity message)
        {
            _agentProvider = agentProvider;
            _botDataStore = botDataStore;
        }

        public async Task<bool> RegisterAgent(IActivity activity)
        {
            var result = _agentProvider.AddAgent(new Agent(activity));
            if(result)
                await SetAgentMetadataInState(Address.FromActivity(activity));
            return result;
        }

        public async Task<bool> UnregisterAgent(IActivity activity)
        {
            var agent = _agentProvider.RemoveAgent(new Agent(activity));
            if (agent == null)
                return false;
            return true;
        }

        public async Task<AgentMetaData> GetAgentMetadata(IAddress agentAddress)
        {
            var botData = await GetBotData(agentAddress, _botDataStore);
            AgentMetaData agentMetaData;
            botData.UserData.TryGetValue(AGENT_METADATA_KEY, out agentMetaData);
            return agentMetaData;
        }

        public async Task StopAgentUserConversation(IAddress userAddress, IAddress agentAddress)
        {
            var userData = await GetBotData(userAddress, _botDataStore);
            var agentData = await GetBotData(agentAddress, _botDataStore);

            userData.PrivateConversationData.RemoveValue(Constants.AGENT_KEY);
            agentData.PrivateConversationData.RemoveValue(Constants.USER_KEY);
        }

        public async Task<User> GetUserFromAgentState(IAddress agentAddress)
        {
            var botData = await GetBotData(agentAddress, _botDataStore);
            User user;
            botData.PrivateConversationData.TryGetValue(Constants.USER_KEY, out user);
            return user;
        }

        public async Task<Agent> GetAgentFromUserState(IAddress userAddress)
        {
            var botData = await GetBotData(userAddress, _botDataStore);
            Agent agent;
            botData.PrivateConversationData.TryGetValue(Constants.AGENT_KEY, out agent);
            return agent;
        }

        private async Task SetAgentMetadataInState(IAddress agentAddress)
        {
            var botData = await GetBotData(agentAddress, _botDataStore);
            botData.UserData.SetValue(AGENT_METADATA_KEY, new AgentMetaData() { IsAgent = true });
            await botData.FlushAsync(CancellationToken.None);
        }
    }
}
