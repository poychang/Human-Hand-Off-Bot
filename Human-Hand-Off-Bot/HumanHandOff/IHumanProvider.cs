using Human_Hand_Off_Bot.HumanHandOff.Models;

namespace Human_Hand_Off_Bot.HumanHandOff
{
    /// <summary>管理服務人員池</summary>
    public interface IHumanProvider
    {
        /// <summary>取得下一位有空的服務人員</summary>
        /// <returns>服務人員</returns>
        Agent GetNextAvailableAgent();

        /// <summary>增加一位指定的服務人員至可用人員池</summary>
        /// <param name="agent">服務人員</param>
        /// <returns>是否成功</returns>
        bool AddAgent(Agent agent);

        /// <summary>從可用人員池移除一位指定的服務人員</summary>
        /// <param name="agent">服務人員</param>
        /// <returns>服務人員</returns>
        Agent RemoveAgent(Agent agent);
    }
}
