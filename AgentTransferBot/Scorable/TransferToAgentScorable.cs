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
    public class TransferToAgentScorable : ScorableBase<IMessageActivity, bool, double>
    {
        private readonly IUserToAgent _agentService;

        public TransferToAgentScorable(IUserToAgent agentService)
        {
            _agentService = agentService;
        }
        protected override Task DoneAsync(IMessageActivity item, bool state, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        protected override double GetScore(IMessageActivity item, bool state)
        {
            return state ? 1.0 : 0;
        }

        protected override bool HasScore(IMessageActivity item, bool state)
        {
            return state;
        }

        protected override async Task PostAsync(IMessageActivity item, bool state, CancellationToken token)
        {
            await _agentService.SendToAgent(item as Activity);
        }

        protected override async Task<bool> PrepareAsync(IMessageActivity item, CancellationToken token)
        {
            return await _agentService.AgentTransferRequired(item as Activity);
        }
    }
}
