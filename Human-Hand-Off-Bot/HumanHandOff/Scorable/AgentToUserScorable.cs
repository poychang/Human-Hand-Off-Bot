using System.Threading;
using System.Threading.Tasks;
using Human_Hand_Off_Bot.HumanHandOff.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;

namespace Human_Hand_Off_Bot.HumanHandOff.Scorable
{
    /// <summary>全域處理程序 - 客服人員→使用者</summary>
    /// <remarks>https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-scorable-dialogs</remarks>
    public class AgentToUserScorable : ScorableBase<IActivity, bool, double>
    {
        private readonly IBotDataStore<BotData> _botDataStore;
        private readonly IBotToUser _botToUser;
        private readonly IHumanService _humanService;

        public AgentToUserScorable(IBotDataStore<BotData> botDataStore, IBotToUser botToUser, IHumanService humanService)
        {
            _botDataStore = botDataStore;
            _botToUser = botToUser;
            _humanService = humanService;
        }

        /// <summary>準備階段：分析對話是否進入此處理程序</summary>
        protected override async Task<bool> PrepareAsync(IActivity item, CancellationToken token) =>
            await _humanService.IsAgent(item, token);

        /// <summary>取得此對話處理程序是否有分數</summary>
        protected override bool HasScore(IActivity item, bool state) => state;

        /// <summary>取得此對話處理程序的分數</summary>
        protected override double GetScore(IActivity item, bool state) => state ? 1.0 : 0.0;

        /// <summary>執行階段：主要的對話處理程序，會從所有全域處理程序中執行最高分的項目</summary>
        protected override async Task PostAsync(IActivity item, bool state, CancellationToken token)
        {
            if (await _humanService.IsInExistingConversationAsync(item, token))
            {
                var message = item as Activity;
                var agentAddress = new Agent(message).GetAddress();
                var botData = await Utility.GetBotDataAsync(agentAddress, _botDataStore, token);
                botData.PrivateConversationData.TryGetValue(Constants.USER_ROUTE_KEY, out Agent agent);

                var reference = agent.ConversationReference;
                var reply = reference.GetPostToUserMessage();
                reply.Text = message?.Text;

                await Utility.SendToConversationAsync(reply);
            }
            else
            {
                await _botToUser.PostAsync("您沒有與任何使用者對話", null, token);
            }
        }

        /// <summary>完成階段：可在此階段清除所占用的資源</summary>
        protected override Task DoneAsync(IActivity item, bool state, CancellationToken token) => Task.CompletedTask;
    }
}
