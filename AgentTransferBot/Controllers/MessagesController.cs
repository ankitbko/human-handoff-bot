using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Autofac;
using System.Threading;

namespace AgentTransferBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await SendAsync(activity, (scope) => new EchoDialog(scope.Resolve<IUserToAgent>()));
            }
            else
            {
                await HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task<Activity> HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }
            else if (message.Type == ActivityTypes.Event)
            {
                using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, message))
                {
                    var agentService = scope.Resolve<IAgentService>();
                    switch (message.AsEventActivity().Name)
                    {
                        case "connect":
                            await agentService.RegisterAgent(message);
                            break;
                        case "stopConversation":
                            await StopConversation(agentService, message);
                            await agentService.RegisterAgent(message);
                            break;
                        default:
                            break;
                    }
                }
            }

            return null;
        }

        private async Task StopConversation(IAgentService agentService, Activity agentActivity)
        {
            var user = await agentService.GetUserFromAgentState(Address.FromActivity(agentActivity));
            var agentReply = agentActivity.CreateReply();
            if (user == null)
            {
                agentReply.Text = "Hey! You were not talking to anyone.";
                await ReplyToActivityAsync(agentReply);
                return;
            }

            var userReply = user.ConversationReference.GetPostToUserMessage();
            await agentService.StopAgentUserConversation(
                Address.FromActivity(userReply),
                Address.FromActivity(agentActivity));

            userReply.Text = "You have been disconnected from our representative.";
            await ReplyToActivityAsync(userReply);
            userReply.Text = "But we can still talk; :)";
            await ReplyToActivityAsync(userReply);

            agentReply.Text = "You have stopped the conversation.";
            await ReplyToActivityAsync(agentReply);
        }

        private async Task ReplyToActivityAsync(Activity activity)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            await connector.Conversations.SendToConversationAsync(activity);
        }

        private async Task SendAsync(IMessageActivity toBot, Func<ILifetimeScope, IDialog<object>> MakeRoot, CancellationToken token = default(CancellationToken))
        {
            using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, toBot))
            {
                DialogModule_MakeRoot.Register(scope, () => MakeRoot(scope));
                var task = scope.Resolve<IPostToBot>();
                await task.PostAsync(toBot, token);
            }
        }
    }
}