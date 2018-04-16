using System.Threading;
using System.Threading.Tasks;
using Human_Hand_Off_Bot.HumanHandOff.Models;
using Microsoft.Bot.Connector;

namespace Human_Hand_Off_Bot.HumanHandOff
{
    public interface IHumanService
    {
        /// <summary>初始化與服務人員連線的對話階段</summary>
        /// <param name="message">活動訊息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<Agent> IntitiateConversationWithAgentAsync(Activity message, CancellationToken cancellationToken);

        /// <summary>判斷是否想要轉接真人</summary>
        /// <param name="message">活動訊息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<bool> IsWantToTalkWithHuman(Activity message, CancellationToken cancellationToken);

        /// <summary>註冊服務人員至服務人員池</summary>
        /// <param name="activity">活動訊息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<bool> RegisterAgentAsync(IActivity activity, CancellationToken cancellationToken);

        /// <summary>註銷服務人員池的服務人員</summary>
        /// <param name="activity">活動訊息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<bool> UnregisterAgentAsync(IActivity activity, CancellationToken cancellationToken);

        /// <summary>是否為服務人員</summary>
        /// <param name="activity">活動訊息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<bool> IsAgent(IActivity activity, CancellationToken cancellationToken);

        /// <summary>是否有已連結的對話階段</summary>
        /// <param name="activity">活動訊息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<bool> IsInExistingConversationAsync(IActivity activity, CancellationToken cancellationToken);

        /// <summary>取得對話階段中的使用者</summary>
        /// <param name="agentActivity">來自服務人員的活動訊息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<User> GetUserInConversationAsync(IActivity agentActivity, CancellationToken cancellationToken);

        /// <summary>取消使用者與服務人員的對話階段</summary>
        /// <param name="userActivity">來自使用者的活動訊息</param>
        /// <param name="agentActivity">來自服務人員的活動訊息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task StopAgentUserConversationAsync(IActivity userActivity, IActivity agentActivity,
           CancellationToken cancellationToken);
    }
}