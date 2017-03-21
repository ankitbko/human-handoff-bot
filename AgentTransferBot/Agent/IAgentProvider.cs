using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentTransferBot
{
    public interface IAgentProvider
    {
        Agent GetNextAvailableAgent();
        bool RegisterAgent();

    }
}
