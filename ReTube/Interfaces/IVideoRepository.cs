using ReTube.Models;
using ReTube.Dtos.Video;
using ReTube.Helpers;

namespace ReTube.Interfaces
{
    public interface IVideoRepository
    {
        Task<List<Video>> GetAllAsync(QueryObject query);
        Task<Video> GetByIdAsync(int id);
        Task<Video> UpdateAsync(int id, UpdateVideoRequestDto videoDto, string userId);
        Task<Video> CreateAsync(CreateVideoRequestDto videoDto, string userName);
        Task<Video> DeleteByIdAsync(int id, string userId);
        Task<bool> GetAnyAsync(int id);
    }
}
