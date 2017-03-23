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
        private ConcurrentQueue<Agent> _availableAgents = new ConcurrentQueue<Agent>();

        public Agent GetNextAvailableAgent()
        {
            Agent agent;
            var success = _availableAgents.TryDequeue(out agent);
            if (success)
                return agent;
            return null;
        }

        public bool AddAgent(Agent agent)
        {
            try
            {
                _availableAgents.Enqueue(agent);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
