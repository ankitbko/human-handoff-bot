using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace AgentTransferBot.Scorable
{
    public class AgentToUserScorable : ScorableBase<IMessageActivity, bool, double>
    {
        private readonly IAgentToUser _agentToUser;

        public AgentToUserScorable(IAgentToUser agentToUser)
        {
            _agentToUser = agentToUser;
        }
        protected override Task DoneAsync(IMessageActivity item, bool state, CancellationToken token) => Task.CompletedTask;

        protected override double GetScore(IMessageActivity item, bool state) => state? 1.0 : 0.0;

        protected override bool HasScore(IMessageActivity item, bool state) => state;

        protected override async Task PostAsync(IMessageActivity item, bool state, CancellationToken token)
        {
            await _agentToUser.SendToUser(item as Activity);
        }

        protected override async Task<bool> PrepareAsync(IMessageActivity item, CancellationToken token)
        {
            return IsAgent(item as Activity);
        }

        private bool IsAgent(Activity activity)
        {
            var data = activity.GetChannelData<AgentChannelData>();
            if (data != null)
                return data.IsAgent;
            return false;
        }
    }
}