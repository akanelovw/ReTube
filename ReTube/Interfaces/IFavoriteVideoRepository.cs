using ReTube.Dtos.Video;
using ReTube.Models;

namespace ReTube.Interfaces
{
    public interface IFavoriteVideoRepository
    {
        Task<List<FavoriteVideo>> GetAllAsync();
        Task<FavoriteVideo> GetByIdAsync(int id);
        Task<FavoriteVideoToVideo> CreateAsync(FavoriteVideoToVideo favoriteVideoToVideoModel);
        Task<FavoriteVideoToVideo> DeleteByIdAsync(int id, string userId);
    }
}
