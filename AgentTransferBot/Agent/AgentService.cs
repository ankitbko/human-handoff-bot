using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System.Threading;
using System.Threading.Tasks;
using static AgentTransferBot.Utilities;
using System;

namespace AgentTransferBot
{
    public class AgentService : IAgentService
    {
        private readonly IAgentProvider _agentProvider;
        private readonly IAgentUserMapping _agentUserMapping;
        private readonly IBotDataStore<BotData> _botDataStore;

        public AgentService(IAgentProvider agentProvider, IAgentUserMapping agentUserMapping, IBotDataStore<BotData> botDataStore)
        {
            _agentProvider = agentProvider;
            _agentUserMapping = agentUserMapping;
            _botDataStore = botDataStore;
        }

        public async Task<bool> IsInExistingConversationAsync(IActivity activity, CancellationToken cancellationToken) =>
            await _agentUserMapping.DoesMappingExist(new Agent(activity), cancellationToken);

        public async Task<bool> RegisterAgentAsync(IActivity activity, CancellationToken cancellationToken)
        {
            var agent = new Agent(activity);
            var result = _agentProvider.AddAgent(agent);
            if(result)
                await StoreAgentMetadataInStateAsync(Address.FromActivity(activity), cancellationToken);
            return result;
        }

        public async Task<bool> UnregisterAgentAsync(IActivity activity, CancellationToken cancellationToken)
        {
            var agent = _agentProvider.RemoveAgent(new Agent(activity));
            var metadata = await RemoveAgentMetadataInStateAsync(Address.FromActivity(activity), cancellationToken);
            if (agent == null)
                return false;
            return metadata;
        }

        public async Task<bool> IsAgent(IActivity activity, CancellationToken cancellationToken)
        {
            var metadata = await GetAgentMetadataAsync(Address.FromActivity(activity), cancellationToken);
            if (metadata == null)
                return false;
            return metadata.IsAgent;
        }

        public async Task StopAgentUserConversationAsync(IActivity userActivity, IActivity agentActivity, CancellationToken cancellationToken)
        {
            await _agentUserMapping.RemoveAgentUserMappingAsync(new Agent(agentActivity), new User(userActivity), cancellationToken);
        }

        public async Task<User> GetUserInConversationAsync(IActivity agentActivity, CancellationToken cancellationToken) => 
            await _agentUserMapping.GetUserFromMappingAsync(new Agent(agentActivity), cancellationToken);

        public async Task<Agent> GetAgentInConversationAsync(IActivity userAcitvity, CancellationToken cancellationToken) => 
            await _agentUserMapping.GetAgentFromMappingAsync(new User(userAcitvity), cancellationToken);

        private async Task StoreAgentMetadataInStateAsync(IAddress agentAddress, CancellationToken cancellationToken)
        {
            var botData = await GetBotDataAsync(agentAddress, _botDataStore, cancellationToken);
            botData.UserData.SetValue(Constants.AGENT_METADATA_KEY, new AgentMetaData() { IsAgent = true });
            await botData.FlushAsync(cancellationToken);
        }
        private async Task<AgentMetaData> GetAgentMetadataAsync(IAddress agentAddress, CancellationToken cancellationToken)
        {
            var botData = await GetBotDataAsync(agentAddress, _botDataStore, cancellationToken);
            AgentMetaData agentMetaData;
            botData.UserData.TryGetValue(Constants.AGENT_METADATA_KEY, out agentMetaData);
            return agentMetaData;
        }
        private async Task<bool> RemoveAgentMetadataInStateAsync(IAddress agentAddress, CancellationToken cancellationToken)
        {
            var botData = await GetBotDataAsync(agentAddress, _botDataStore, cancellationToken);
            var success = botData.UserData.RemoveValue(Constants.AGENT_METADATA_KEY);
            await botData.FlushAsync(cancellationToken);
            return success;
        }
    }
}
