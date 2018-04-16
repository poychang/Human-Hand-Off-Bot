using System;
using System.Threading;
using System.Threading.Tasks;
using Human_Hand_Off_Bot.HumanHandOff;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Human_Hand_Off_Bot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private readonly IHumanService _humanService;

        public RootDialog(IHumanService humanService)
        {
            _humanService = humanService;
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (await _humanService.IsWantToTalkWithHuman(activity, default(CancellationToken)))
            {
                var agent = await _humanService.IntitiateConversationWithAgentAsync(activity, default(CancellationToken));
                if (agent == null)
                    await context.PostAsync("所有客服人員皆在忙線中，請您稍後再試，謝謝");
            }
            else
            {
                // calculate something for us to return
                var length = (activity.Text ?? string.Empty).Length;

                // return our reply to the user
                await context.PostAsync($"You sent {activity.Text} which was {length} characters");

                context.Wait(MessageReceivedAsync);
            }

            context.Done(true);
        }
    }
}