using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder.Dialogs;

namespace AgentTransferBot.Scorable
{
    public class AgentToUserScorable : ScorableBase<IActivity, bool, double>
    {
        private readonly IAgentService _agentService;
        private readonly IAgentToUser _agentToUser;

        public AgentToUserScorable(IAgentToUser agentToUser, IAgentService agentService)
        {
            _agentToUser = agentToUser;
            _agentService = agentService;
        }
        protected override Task DoneAsync(IActivity item, bool state, CancellationToken token) => Task.CompletedTask;

        protected override double GetScore(IActivity item, bool state) => state? 1.0 : 0.0;

        protected override bool HasScore(IActivity item, bool state) => state;

        protected override async Task PostAsync(IActivity item, bool state, CancellationToken token)
        {
            await _agentToUser.SendToUser(item as Activity);
        }

        protected override async Task<bool> PrepareAsync(IActivity item, CancellationToken token)
        {
            return await IsAgent(item);
        }

        private async Task<bool> IsAgent(IActivity activity)
        {
            var agentData = await _agentService.GetAgentMetadata(Address.FromActivity(activity));
            if (agentData != null)
                return agentData.IsAgent;
            return false;
        }
    }
}