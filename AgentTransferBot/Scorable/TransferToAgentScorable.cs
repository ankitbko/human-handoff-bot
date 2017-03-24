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
    public class TransferToAgentScorable : ScorableBase<IActivity, bool, double>
    {
        private readonly IUserToAgent _agentService;

        public TransferToAgentScorable(IUserToAgent agentService)
        {
            _agentService = agentService;
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
            await _agentService.SendToAgent(item as Activity, token);
        }

        protected override async Task<bool> PrepareAsync(IActivity item, CancellationToken token)
        {
            return await _agentService.AgentTransferRequired(item as Activity, token);
        }
    }
}
