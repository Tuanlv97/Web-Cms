using System.Threading.Tasks;

namespace Cms.Api.Services.Interfaces
{
    public interface ISequenceService
    {
        Task<int> GetKnowledgeBaseNewId();
    }
}
