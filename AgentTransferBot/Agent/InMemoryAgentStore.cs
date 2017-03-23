using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentTransferBot
{
    public class InMemoryAgentStore : IAgentProvider
    {
        private ConcurrentDictionary<string, Agent> _availableAgents = new ConcurrentDictionary<string, Agent>();
        private static object objectLock = new object();

        public Agent GetNextAvailableAgent()
        {
            Agent agent = null;

            lock (objectLock)
            {
                if (_availableAgents.Count > 0)
                {
                    var key = _availableAgents.Keys.First();
                    agent = RemoveAgent(key);
                }
            }
            return agent;
        }

        public bool AddAgent(Agent agent)
        {
            try
            {
                _availableAgents.AddOrUpdate(agent.AgentId, agent, (k, v) => agent);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Agent RemoveAgent(Agent agent)
        {
            lock (objectLock)
            {
                return RemoveAgent(agent.AgentId);
            }
        }

        private Agent RemoveAgent(string id)
        {
            Agent res;
            _availableAgents.TryRemove(id, out res);
            return res;
        }
    }
}
