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
    public static class Utilities
    {
        public static async Task SendToConversationAsync(Activity activity)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            await connector.Conversations.SendToConversationAsync(activity);
        }

        public static async Task<IBotData> GetBotData(IAddress userAddress, IBotDataStore<BotData> botDataStore, CancellationToken cancellationToken)
        {
            var botData = new JObjectBotData(userAddress, botDataStore);
            await botData.LoadAsync(cancellationToken);
            return botData;
        }
    }
}
