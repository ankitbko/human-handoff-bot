using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;

namespace AgentTransferBot.Scorable
{
    public class AgentToUserScorable : ScorableBase<IActivity, bool, double>
    {
        private readonly IAgentService _agentService;
        private readonly IAgentToUser _agentToUser;
        private readonly IBotToUser _botToUser;

        public AgentToUserScorable(IAgentToUser agentToUser, IAgentService agentService, IBotToUser botToUser)
        {
            _agentToUser = agentToUser;
            _agentService = agentService;
            _botToUser = botToUser;
        }
        protected override Task DoneAsync(IActivity item, bool state, CancellationToken token) => Task.CompletedTask;

        protected override double GetScore(IActivity item, bool state) => state? 1.0 : 0.0;

        protected override bool HasScore(IActivity item, bool state) => state;

        protected override async Task PostAsync(IActivity item, bool state, CancellationToken token)
        {
            if (await _agentService.IsInExistingConversationAsync(item, token))
                await _agentToUser.SendToUserAsync(item as Activity, token);
            else
            {
                await _botToUser.PostAsync("You are not talking with any user.");
                await _botToUser.PostAsync("And sadly, you can't talk to me either. :(");
            }
        }

        protected override async Task<bool> PrepareAsync(IActivity item, CancellationToken token) =>
            await _agentService.IsAgent(item, token);
    }
}