using System.Threading;
using System.Threading.Tasks;
using Human_Hand_Off_Bot.HumanHandOff.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;

namespace Human_Hand_Off_Bot.HumanHandOff
{
    public class HumanService : IHumanService
    {
        private readonly IBotDataStore<BotData> _botDataStore;
        private readonly IHumanProvider _humanProvider;

        public HumanService(IBotDataStore<BotData> botDataStore, IHumanProvider humanProvider)
        {
            _botDataStore = botDataStore;
            _humanProvider = humanProvider;
        }

        /// <inheritdoc />
        public async Task<Agent> IntitiateConversationWithAgentAsync(Activity message, CancellationToken cancellationToken)
        {
            var user = new User(message);
            var agent = _humanProvider.GetNextAvailableAgent();
            if (agent == null)
                return null;

            var agentAddress = agent.GetAddress();
            var userAddress = user.GetAddress();

            var botDataOfUser = await Utility.GetBotDataAsync(userAddress, _botDataStore, cancellationToken);
            botDataOfUser.PrivateConversationData.SetValue(Constants.AGENT_ROUTE_KEY, agent);
            await botDataOfUser.FlushAsync(cancellationToken);

            var botDataOfAgent = await Utility.GetBotDataAsync(agentAddress, _botDataStore, cancellationToken);
            botDataOfAgent.PrivateConversationData.SetValue(Constants.USER_ROUTE_KEY, user);
            await botDataOfAgent.FlushAsync(cancellationToken);

            var userReply = message.CreateReply($"您已與 {agent.ConversationReference.User.Name} 完成連線，請開始對話");
            await Utility.SendToConversationAsync(userReply);

            var agentReply = agent.ConversationReference.GetPostToUserMessage();
            agentReply.Text = $"{message.From.Name} 加入這次對話";
            await Utility.SendToConversationAsync(agentReply);

            return agent;
        }

        /// <inheritdoc />
        public Task<bool> IsWantToTalkWithHuman(Activity message, CancellationToken cancellationToken)
        {
            return Task.FromResult(message.Text.Contains("human"));
        }

        /// <inheritdoc />
        public async Task<bool> RegisterAgentAsync(IActivity activity, CancellationToken cancellationToken)
        {
            var agent = new Agent(activity);
            var result = _humanProvider.AddAgent(agent);

            if (!result) return false;

            var botData = await Utility.GetBotDataAsync(Address.FromActivity(activity), _botDataStore, cancellationToken);
            botData.UserData.SetValue(Constants.AGENT_METADATA_KEY, new AgentMetaData() { IsAgent = true });
            await botData.FlushAsync(cancellationToken);

            return true;
        }

        /// <inheritdoc />
        public async Task<bool> UnregisterAgentAsync(IActivity activity, CancellationToken cancellationToken)
        {
            var agent = new Agent(activity);
            _humanProvider.RemoveAgent(agent);

            var botData = await Utility.GetBotDataAsync(Address.FromActivity(activity), _botDataStore, cancellationToken);
            var success = botData.UserData.RemoveValue(Constants.AGENT_METADATA_KEY);
            await botData.FlushAsync(cancellationToken);

            return success;
        }

        /// <inheritdoc />
        public async Task<bool> IsAgent(IActivity activity, CancellationToken cancellationToken)
        {
            var botData = await Utility.GetBotDataAsync(Address.FromActivity(activity), _botDataStore, cancellationToken);
            botData.UserData.TryGetValue(Constants.AGENT_METADATA_KEY, out AgentMetaData agentMetaData);
            return agentMetaData != null && agentMetaData.IsAgent;
        }

        /// <inheritdoc />
        public async Task<bool> IsInExistingConversationAsync(IActivity activity, CancellationToken cancellationToken)
        {
            var agentAddress = new Agent(activity).GetAddress();
            var botData = await Utility.GetBotDataAsync(agentAddress, _botDataStore, cancellationToken);
            return botData.PrivateConversationData.ContainsKey(Constants.USER_ROUTE_KEY);
        }

        /// <inheritdoc />
        public async Task<User> GetUserInConversationAsync(IActivity agentActivity, CancellationToken cancellationToken)
        {
            var agent = new Agent(agentActivity);
            var agentAddress = agent.GetAddress();
            var botData = await Utility.GetBotDataAsync(agentAddress, _botDataStore, cancellationToken);
            botData.PrivateConversationData.TryGetValue(Constants.USER_ROUTE_KEY, out User user);

            return user;
        }

        /// <inheritdoc />
        public async Task StopAgentUserConversationAsync(IActivity userActivity, IActivity agentActivity,
            CancellationToken cancellationToken)
        {
            var userAddress = new User(userActivity).GetAddress();
            var agentAddress = new Agent(agentActivity).GetAddress();

            var userData = await Utility.GetBotDataAsync(userAddress, _botDataStore, cancellationToken);
            var agentData = await Utility.GetBotDataAsync(agentAddress, _botDataStore, cancellationToken);

            userData.PrivateConversationData.RemoveValue(Constants.AGENT_ROUTE_KEY);
            agentData.PrivateConversationData.RemoveValue(Constants.USER_ROUTE_KEY);

            await userData.FlushAsync(cancellationToken);
            await agentData.FlushAsync(cancellationToken);
        }

    }
}