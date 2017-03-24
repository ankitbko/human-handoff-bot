using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using static AgentTransferBot.Utilities;

namespace AgentTransferBot
{
    public class UserToAgentRouter : IUserToAgent
    {
        private readonly IAgentProvider _agentProvider;
        private readonly IAgentService _agentService;
        private readonly IBotDataStore<BotData> _botDataStore;

        public UserToAgentRouter(IAgentProvider agentProvider, IAgentService agentService, IBotDataStore<BotData> botDataStore)
        {
            _botDataStore = botDataStore;
            _agentProvider = agentProvider;
            _agentService = agentService;
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
            await SendToConversationAsync(userReply);

            var agentReply = agent.ConversationReference.GetPostToUserMessage();
            agentReply.Text = $"{message.From.Name} has joined the conversation.";
            await SendToConversationAsync(agentReply);

            return agent;
        }

        public async Task SendToAgent(Activity message)
        {
            var agent = await _agentService.GetAgentFromUserState(Address.FromActivity(message));
            var reference = agent.ConversationReference;
            var reply = reference.GetPostToUserMessage();
            reply.Text = message.Text;

            await SendToConversationAsync(reply);
        }

        #region Private Members
        private async Task<bool> IsInExistingConversationWithAgent(Activity message)
        {
            var botData = await GetBotData(Address.FromActivity(message), _botDataStore);
            return botData.PrivateConversationData.ContainsKey(Constants.AGENT_KEY);
        }
        private async Task SetAgentToUserState(IAddress userAddress, Agent agent)
        {
            var botData = await GetBotData(userAddress, _botDataStore);
            botData.PrivateConversationData.SetValue(Constants.AGENT_KEY, agent);
            await botData.FlushAsync(CancellationToken.None);
        }
        private async Task SetUserToAgentState(Agent agent, User user)
        {
            var botData = await GetBotData(Address.FromActivity(agent.ConversationReference.GetPostToBotMessage()), _botDataStore);
            botData.PrivateConversationData.SetValue(Constants.USER_KEY, user);
            await botData.FlushAsync(CancellationToken.None);
        }
        #endregion
    }
}
