using Microsoft.EntityFrameworkCore;
using ReTube.Data;
using ReTube.Interfaces;
using ReTube.Models;

namespace ReTube.Repository
{
    public class PlaylistVideoRepository : IPlaylistVideoRepository
    {
        private readonly ApplicationDbContext _context;
        public PlaylistVideoRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PlaylistVideo> CreateAsync(PlaylistVideo playlistVideoModel)
        {
            await _context.PlaylistVideo.AddAsync(playlistVideoModel);
            await _context.SaveChangesAsync();
            return playlistVideoModel;
        }
        public async Task<PlaylistVideo> DeleteAsync(int videoId, int playlistId, string userId)
        {

            var playlistVideo = await _context.PlaylistVideo.FirstOrDefaultAsync(a => a.VideoId == videoId && a.PlaylistId == playlistId);
            var playlist = await _context.Playlist.SingleOrDefaultAsync(a => a.Id == playlistId);

            if (playlistVideo == null)
            {
                return null;
            }

            if (userId != playlist.UserId)
            {
                throw new Exception("You have no rights to delete this post");
            }
            _context.PlaylistVideo.Remove(playlistVideo);
            await _context.SaveChangesAsync();
            return playlistVideo;
        }
    }
}
