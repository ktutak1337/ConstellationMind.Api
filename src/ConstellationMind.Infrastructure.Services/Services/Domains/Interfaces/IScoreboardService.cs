using System.Collections.Generic;
using System.Threading.Tasks;
using ConstellationMind.Infrastructure.Services.DTO;
using ConstellationMind.Shared.Types;

namespace ConstellationMind.Infrastructure.Services.Domains.Interfaces
{
    public interface IScoreboardService : IService
    {
        Task<IEnumerable<PlayerScoreDto>> GetScoreboardAsync();
    }
}
