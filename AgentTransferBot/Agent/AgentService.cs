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

namespace AgentTransferBot
{
    public class AgentService : IAgentService, IUserToAgent, IAgentToUser
    {
        private const string AGENT_KEY = "AgentRouteKey";
        private const string USER_KEY = "UserRouteKey";
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
            var result = _agentProvider.RegisterAgent(new Agent(activity));
            if(result)
                await SetAgentMetadataInState(Address.FromActivity(activity));
            return result;
        }

        public async Task<AgentMetaData> GetAgentMetadata(IAddress agentAddress)
        {
            var botData = await GetBotData(agentAddress);
            AgentMetaData agentMetaData;
            var success = botData.UserData.TryGetValue(AGENT_METADATA_KEY, out agentMetaData);
            if (success)
                return agentMetaData;
            return null;
        }

        public async Task<bool> AgentTransferRequired(Activity message)
        {
            // TODO && Check if it is valid conversation. eg. it is within last 5 min
            return await IsInExistingConversationWithAgent(message);
        }

        public async Task<Agent> IntitiateConversationWithAgent(Activity message)
        {
            var agent = _agentProvider.GetNextAvailableAgent();
            if (agent == null)
                return null;

            await SetAgentToUserState(Address.FromActivity(message), agent);
            await SetUserToAgentState(agent, new User(message));

            var userReply = message.CreateReply($"You are now connected to {agent.ConversationReference.User.Name}");
            await ReplyToActivityAsync(userReply);

            var agentReply = agent.ConversationReference.GetPostToUserMessage();
            agentReply.Text = $"{message.From.Name} has joined the conversation.";
            await ReplyToActivityAsync(agentReply);

            return agent;
        }

        public async Task StopAgentUserConversation(IAddress userAddress, IAddress agentAddress)
        {
            var userData = await GetBotData(userAddress);
            var agentData = await GetBotData(agentAddress);

            userData.PrivateConversationData.RemoveValue(AGENT_KEY);
            agentData.PrivateConversationData.RemoveValue(USER_KEY);
        }

        public async Task SendToAgent(Activity message)
        {
            var agent = await GetAgentFromUserState(Address.FromActivity(message));
            var reference = agent.ConversationReference;
            var reply = reference.GetPostToUserMessage();
            reply.Text = message.Text;

            await ReplyToActivityAsync(reply);
        }

        public async Task SendToUser(Activity message)
        {
            var user = await GetUserFromAgentState(Address.FromActivity(message));
            var reference = user.ConversationReference;
            var reply = reference.GetPostToUserMessage();
            reply.Text = message.Text;

            await ReplyToActivityAsync(reply);
        }

        public async Task<User> GetUserFromAgentState(IAddress agentAddress)
        {
            var botData = await GetBotData(agentAddress);
            return botData.PrivateConversationData.Get<User>(USER_KEY);
        }

        public async Task<Agent> GetAgentFromUserState(IAddress userAddress)
        {
            var botData = await GetBotData(userAddress);
            return botData.PrivateConversationData.Get<Agent>(AGENT_KEY);
        }

        private async Task<bool> IsInExistingConversationWithAgent(Activity message)
        {
            var botData = await GetBotData(Address.FromActivity(message));
            return botData.PrivateConversationData.ContainsKey(AGENT_KEY);
        }

        private async Task SetAgentToUserState(IAddress userAddress, Agent agent)
        {
            var botData = await GetBotData(userAddress);
            botData.PrivateConversationData.SetValue(AGENT_KEY, agent);
            await botData.FlushAsync(CancellationToken.None);
        }

        private async Task ReplyToActivityAsync(Activity activity)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            await connector.Conversations.SendToConversationAsync(activity);
        }

        private async Task SetUserToAgentState(Agent agent, User user)
        {
            var botData = await GetBotData(Address.FromActivity(agent.ConversationReference.GetPostToBotMessage()));
            botData.PrivateConversationData.SetValue(USER_KEY, user);
            await botData.FlushAsync(CancellationToken.None);
        }

        private async Task<IBotData> GetBotData(IAddress userAddress)
        {
            var botData = new JObjectBotData(userAddress, _botDataStore);
            await botData.LoadAsync(default(CancellationToken));
            return botData;
        }

        private async Task SetAgentMetadataInState(IAddress agentAddress)
        {
            var botData = await GetBotData(agentAddress);
            botData.UserData.SetValue(AGENT_METADATA_KEY, new AgentMetaData() { IsAgent = true });
            await botData.FlushAsync(CancellationToken.None);
        }
    }
}
