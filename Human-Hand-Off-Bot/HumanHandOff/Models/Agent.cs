using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Human_Hand_Off_Bot.HumanHandOff.Models
{
    /// <summary>服務人員</summary>
    public class Agent
    {
        public Agent()
        {
        }

        public Agent(IActivity activity)
        {
            ConversationReference = activity.ToConversationReference();
            AgentId = activity.From.Id;
        }

        /// <summary>服務人員識別碼</summary>
        public string AgentId { get; set; }

        /// <summary>對話框的參考</summary>
        public ConversationReference ConversationReference { get; set; }

        /// <summary>取得服務人員在某個對話階段的識別碼</summary>
        /// <returns></returns>
        public IAddress GetAddress() => Address.FromActivity(ConversationReference.GetPostToBotMessage());
    }
}