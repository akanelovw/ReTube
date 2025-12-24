using Microsoft.EntityFrameworkCore;
using ReTube.Data;
using ReTube.Dtos.Video;
using ReTube.Interfaces;
using ReTube.Models;


namespace ReTube.Repository
{
    public class FavoriteVideoRepository : IFavoriteVideoRepository
    {
        private readonly ApplicationDbContext _context;
        public FavoriteVideoRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<FavoriteVideo>> GetAllAsync()
        {
            return await _context.FavoriteVideo.Include(a => a.User).Include(a => a.Videos).ToListAsync();
        }
        public async Task<FavoriteVideo> GetByIdAsync(int id)
        {
            return await _context.FavoriteVideo.Include(a => a.User).Include(a => a.Videos).FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<FavoriteVideoToVideo> CreateAsync(FavoriteVideoToVideo favoriteVideoToVideoModel)
        {
            await _context.FavoriteVideoToVideo.AddAsync(favoriteVideoToVideoModel);
            await _context.SaveChangesAsync();
            return favoriteVideoToVideoModel;
        }


        public async Task<FavoriteVideoToVideo> DeleteByIdAsync(int id, string userId)
        {

            var favoriteVideo = await _context.FavoriteVideo.SingleOrDefaultAsync(a => a.UserId == userId);
            var favoriteVideoToVideo = await _context.FavoriteVideoToVideo.FirstOrDefaultAsync(a => a.VideoId == id && a.FavoriteVideoId == favoriteVideo.Id);

            if (favoriteVideoToVideo == null)
            {
                return null;
            }

            if (userId != favoriteVideo.UserId)
            {
                throw new Exception("You have no rights to delete this post");
            }
            _context.FavoriteVideoToVideo.Remove(favoriteVideoToVideo);
            await _context.SaveChangesAsync();
            return favoriteVideoToVideo;
        }
    }
}