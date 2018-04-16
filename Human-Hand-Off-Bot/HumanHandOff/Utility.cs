using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;

namespace Human_Hand_Off_Bot.HumanHandOff
{
    public static class Utility
    {
        /// <summary>傳送訊息至指定的對話階段</summary>
        /// <param name="activity">活動訊息</param>
        /// <returns></returns>
        public static async Task SendToConversationAsync(Activity activity)
        {
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            await connector.Conversations.SendToConversationAsync(activity);
        }

        /// <summary>取得機器人詳細資訊</summary>
        /// <param name="userAddress">某個對話階段的識別碼</param>
        /// <param name="botDataStore">機器人詳細資訊的儲存庫</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public static async Task<IBotData> GetBotDataAsync(IAddress userAddress, IBotDataStore<BotData> botDataStore, CancellationToken cancellationToken)
        {
            var botData = new JObjectBotData(userAddress, botDataStore);
            await botData.LoadAsync(cancellationToken);
            return botData;
        }
    }
}