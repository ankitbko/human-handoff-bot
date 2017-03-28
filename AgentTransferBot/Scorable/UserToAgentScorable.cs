using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder.Scorables.Internals;

namespace AgentTransferBot
{
    public class UserToAgentScorable : ScorableBase<IActivity, bool, double>
    {
        private readonly IUserToAgent _userToAgent;

        public UserToAgentScorable(IUserToAgent userToAgent)
        {
            _userToAgent = userToAgent;
        }
        protected override Task DoneAsync(IActivity item, bool state, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        protected override double GetScore(IActivity item, bool state)
        {
            return state ? 1.0 : 0;
        }

        protected override bool HasScore(IActivity item, bool state)
        {
            return state;
        }

        protected override async Task PostAsync(IActivity item, bool state, CancellationToken token)
        {
            await _userToAgent.SendToAgentAsync(item as Activity, token);
        }

        protected override async Task<bool> PrepareAsync(IActivity item, CancellationToken token)
        {
            return await _userToAgent.AgentTransferRequiredAsync(item as Activity, token);
        }
    }
}
