using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentTransferBot
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        private readonly IUserToAgent _userToAgent;

        public EchoDialog(IUserToAgent userToAgent)
        {
            _userToAgent = userToAgent;
        }
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceived);
        }

        private async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            if (message.Text.StartsWith("a"))
            {
                var agent = await _userToAgent.IntitiateConversationWithAgent(message as Activity);
                if (agent == null)
                    await context.PostAsync("All our customer care representatives are busy at the moment. Please try after some time.");
            }
            else
            {
                await context.PostAsync(message.Text);
            }
            context.Done(true);
        }
    }
}
