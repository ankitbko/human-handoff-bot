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
        private readonly IAgentProvider _agentProvider;
        private readonly IBotDataStore<BotData> _botDataStore;

        public AgentService(IAgentProvider agentProvider, IBotDataStore<BotData> botDataStore, IActivity message)
        {
            _agentProvider = agentProvider;
            _botDataStore = botDataStore;
        }

        public bool AgentTransferRequired(Activity message)
        {
            return IsInExistingConversationWithAgent(message);
        }

        public async Task<Agent> IntitiateConversationWithAgent(Activity message)
        {
            var agent = _agentProvider.GetNextAvailableAgent();
            if (agent == null)
                return null;

            await SetAgentToUserState(Address.FromActivity(message), agent);
            await SetUserToAgentState(agent, new User() { ConversationReference = message.ToConversationReference() });

            var userReply = message.CreateReply($"You are now connected to {agent.ConversationReference.User.Name}");
            await ReplyToActivityAsync(userReply);

            var agentReply = agent.ConversationReference.GetPostToUserMessage();
            agentReply.Text = $"{message.From.Name} has joined the conversation.";
            await ReplyToActivityAsync(agentReply);

            return agent;
        }

        public void StopAgentUserConversation(IAddress userAddress, IAddress agentAddress)
        {
            var userData = GetBotData(userAddress);
            var agentData = GetBotData(agentAddress);

            userData.PrivateConversationData.RemoveValue(AGENT_KEY);
            agentData.PrivateConversationData.RemoveValue(USER_KEY);
        }

        public async Task SendToAgent(Activity message)
        {
            var agent = GetAgentFromUserState(Address.FromActivity(message));
            var reference = agent.ConversationReference;
            var reply = reference.GetPostToUserMessage();
            reply.Text = message.Text;

            await ReplyToActivityAsync(message);
        }

        public async Task SendToUser(Activity message)
        {
            var user = GetUserFromAgentState(Address.FromActivity(message));
            var reference = user.ConversationReference;
            var reply = reference.GetPostToUserMessage();
            reply.Text = message.Text;

            await ReplyToActivityAsync(message);
        }

        public User GetUserFromAgentState(IAddress agentAddress)
        {
            var botData = GetBotData(agentAddress);
            return botData.PrivateConversationData.Get<User>(USER_KEY);
        }

        public Agent GetAgentFromUserState(IAddress userAddress)
        {
            var botData = GetBotData(userAddress);
            return botData.PrivateConversationData.Get<Agent>(AGENT_KEY);
        }

        private bool IsInExistingConversationWithAgent(Activity message)
        {
            var botData = GetBotData(Address.FromActivity(message));
            return botData.PrivateConversationData.ContainsKey(AGENT_KEY);
        }

        private async Task SetAgentToUserState(IAddress userAddress, Agent agent)
        {
            var botData = GetBotData(userAddress);
            botData.PrivateConversationData.SetValue(AGENT_KEY, agent);
            await botData.FlushAsync(CancellationToken.None);
        }

        private async Task ReplyToActivityAsync(Activity activity)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            await connector.Conversations.ReplyToActivityAsync(activity);
        }

        private async Task SetUserToAgentState(Agent agent, User user)
        {
            var botData = GetBotData(Address.FromActivity(agent.ConversationReference.GetPostToBotMessage()));
            botData.PrivateConversationData.SetValue(USER_KEY, user);
            await botData.FlushAsync(CancellationToken.None);
        }

        private IBotData GetBotData(IAddress userAddress)
        {
            return new JObjectBotData(userAddress, _botDataStore);
        }

    }
}
