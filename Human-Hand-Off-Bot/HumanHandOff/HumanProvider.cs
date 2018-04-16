using System;
using System.Collections.Concurrent;
using System.Linq;
using Human_Hand_Off_Bot.HumanHandOff.Models;

namespace Human_Hand_Off_Bot.HumanHandOff
{
    public class HumanProvider : IHumanProvider
    {
        private static readonly object ObjectLock = new object();
        private readonly ConcurrentDictionary<string, Agent> _availableAgents = new ConcurrentDictionary<string, Agent>();

        /// <inheritdoc/>
        public Agent GetNextAvailableAgent()
        {
            Agent agent;
            lock (ObjectLock)
            {
                if (_availableAgents.Count <= 0) return null;

                var agentId = _availableAgents.Keys.First();
                _availableAgents.TryRemove(agentId, out agent);
            }
            return agent;
        }

        /// <inheritdoc/>
        public bool AddAgent(Agent agent)
        {
            try
            {
                lock (ObjectLock)
                {
                    _availableAgents.AddOrUpdate(agent.AgentId, agent, (k, v) => agent);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public Agent RemoveAgent(Agent agent)
        {
            lock (ObjectLock)
            {
                _availableAgents.TryRemove(agent.AgentId, out var resource);
                return resource;
            }
        }
    }
}