using ReTube.Models;

namespace ReTube.Interfaces
{
    public interface IPlaylistVideoRepository
    {
        Task<PlaylistVideo> CreateAsync(PlaylistVideo playlistVideoModel);
        Task<PlaylistVideo> DeleteAsync(int videoId, int playlistId, string userId);
    }
}
