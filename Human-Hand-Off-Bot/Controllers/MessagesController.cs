using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Human_Hand_Off_Bot.Dialogs;
using Human_Hand_Off_Bot.HumanHandOff;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;

namespace Human_Hand_Off_Bot.Controllers
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
                await SendAsync(activity, (scope) => new RootDialog(scope.Resolve<IHumanService>()));
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
                // Handle knowing that the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
                // ping
            }
            else if (message.Type == ActivityTypes.Event)
            {
                using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, message))
                {
                    var cancellationToken = default(CancellationToken);
                    var humanService = scope.Resolve<IHumanService>();
                    switch (message.AsEventActivity().Name)
                    {
                        case "connect": // 註冊連線
                            await humanService.RegisterAgentAsync(message, cancellationToken);
                            break;

                        case "disconnect": // 中斷連線
                            await humanService.UnregisterAgentAsync(message, cancellationToken);
                            break;

                        case "stopConversation": // 停止對話
                            await StopConversation(humanService, message, cancellationToken);
                            await humanService.RegisterAgentAsync(message, cancellationToken);
                            break;
                    }
                }
            }

            return null;
        }

        private async Task StopConversation(IHumanService humanService, Activity agentActivity, CancellationToken cancellationToken)
        {
            var user = await humanService.GetUserInConversationAsync(agentActivity, cancellationToken);
            var agentReply = agentActivity.CreateReply();

            if (user == null)
            {
                agentReply.Text = "您的訊息沒有送給任何人唷...";
                await Utility.SendToConversationAsync(agentReply);
                return;
            }

            var userActivity = user.ConversationReference.GetPostToBotMessage();
            await humanService.StopAgentUserConversationAsync(userActivity, agentActivity, cancellationToken);

            var userReply = userActivity.CreateReply();
            userReply.Text = "您已與客服人員結束連線";
            await Utility.SendToConversationAsync(userReply);

            agentReply.Text = "您已停止對話";
            await Utility.SendToConversationAsync(agentReply);
        }

        private async Task SendAsync(IMessageActivity toBot, Func<ILifetimeScope, IDialog<object>> makeRoot, CancellationToken token = default(CancellationToken))
        {
            using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, toBot))
            {
                DialogModule_MakeRoot.Register(scope, () => makeRoot(scope));
                var task = scope.Resolve<IPostToBot>();
                await task.PostAsync(toBot, token);
            }
        }
    }
}