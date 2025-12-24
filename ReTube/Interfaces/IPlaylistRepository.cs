using ReTube.Dtos.Playlist;
using ReTube.Helpers;
using ReTube.Models;

namespace ReTube.Interfaces
{
    public interface IPlaylistRepository
    {
        Task<List<Playlist>> GetAllAsync(QueryObject query);
        Task<Playlist> GetByIdAsync(int id);
        Task<Playlist> CreateAsync(Playlist playlistModel);
        Task<Playlist> UpdateAsync(int id, CreatePlaylistRequestDto playlistDto, string userId, IWebHostEnvironment _webHostEnvironment);
        Task<Playlist> DeleteByIdAsync(int id, string userId);
    }
}
