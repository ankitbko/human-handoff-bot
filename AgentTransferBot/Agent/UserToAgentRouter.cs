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
        private readonly IAgentUserMapping _agentUserMapping;

        public UserToAgentRouter(IAgentProvider agentProvider, IAgentService agentService, IAgentUserMapping agentUserMapping)
        {
            _agentProvider = agentProvider;
            _agentService = agentService;
            _agentUserMapping = agentUserMapping;
        }

        public async Task<bool> AgentTransferRequiredAsync(Activity message, CancellationToken cancellationToken)
        {
            // TODO && Check if it is valid conversation. eg. it is within last 5 min
            return await IsInExistingConversationWithAgentAsync(message, cancellationToken);
        }

        public async Task<Agent> IntitiateConversationWithAgentAsync(Activity message, CancellationToken cancellationToken)
        {
            var agent = _agentProvider.GetNextAvailableAgent();
            if (agent == null)
                return null;

            await _agentUserMapping.SetAgentUserMappingAsync(agent, new User(message), cancellationToken);

            var userReply = message.CreateReply($"You are now connected to {agent.ConversationReference.User.Name}");
            await SendToConversationAsync(userReply);

            var agentReply = agent.ConversationReference.GetPostToUserMessage();
            agentReply.Text = $"{message.From.Name} has joined the conversation.";
            await SendToConversationAsync(agentReply);

            return agent;
        }

        public async Task SendToAgentAsync(Activity message, CancellationToken cancellationToken)
        {
            var agent = await _agentService.GetAgentInConversationAsync(message, cancellationToken);
            var reference = agent.ConversationReference;
            var reply = reference.GetPostToUserMessage();
            reply.Text = message.Text;

            await SendToConversationAsync(reply);
        }

        private async Task<bool> IsInExistingConversationWithAgentAsync(Activity message, CancellationToken cancellationToken) => 
            await _agentUserMapping.DoesMappingExist(new User(message), cancellationToken);
    }
}
