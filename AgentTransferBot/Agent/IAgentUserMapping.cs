using System.Threading;
using System.Threading.Tasks;

namespace AgentTransferBot
{
    public interface IAgentUserMapping
    {
        Task<Agent> GetAgentFromMappingAsync(User user, CancellationToken cancellationToken);
        Task<User> GetUserFromMappingAsync(Agent agent, CancellationToken cancellationToken);
        Task SetAgentUserMappingAsync(Agent agent, User user, CancellationToken cancellationToken);
        Task RemoveAgentUserMappingAsync(Agent agent, User user, CancellationToken cancellationToken);
        Task<bool> DoesMappingExist(Agent agent, CancellationToken cancellationToken);
        Task<bool> DoesMappingExist(User user, CancellationToken cancellationToken);
    }
}
