using System;
using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Human_Hand_Off_Bot.HumanHandOff.Models
{
    /// <summary>使用者</summary>
    [Serializable]
    public class User
    {
        public User()
        {
        }

        public User(IActivity message)
        {
            ConversationReference = message.ToConversationReference();
        }

        /// <summary>對話框的參考</summary>
        public ConversationReference ConversationReference { get; set; }

        /// <summary>取得使用者在某個對話階段的識別碼</summary>
        /// <returns></returns>
        public IAddress GetAddress() => Address.FromActivity(ConversationReference.GetPostToBotMessage());
    }
}