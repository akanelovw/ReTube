using Microsoft.EntityFrameworkCore;
using ReTube.Data;
using ReTube.Dtos.Playlist;
using ReTube.Dtos.Video;
using ReTube.Helpers;
using ReTube.Interfaces;
using ReTube.Models;
using ReTube.Service;

namespace ReTube.Repository
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly ApplicationDbContext _context;
        public PlaylistRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Playlist>> GetAllAsync(QueryObject query)
        {
            var playlists = _context.Playlist.Include(a => a.User).Include(a => a.Videos).AsQueryable();
            if (!string.IsNullOrWhiteSpace(query.Title))
            {
                playlists = playlists.Where(v => v.Title.Contains(query.Title));
            }
            if (!string.IsNullOrWhiteSpace(query.Username))
            {
                playlists = playlists.Where(v => v.User.UserName.Contains(query.Username));
            }
            if (string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Title", StringComparison.OrdinalIgnoreCase))
                {
                    playlists = query.IsDecsending ? playlists.OrderByDescending(t => t.Title) : playlists.OrderBy(t => t.Title);
                }
                if (query.SortBy.Equals("Username", StringComparison.OrdinalIgnoreCase))
                {
                    playlists = query.IsDecsending ? playlists.OrderByDescending(t => t.User.UserName) : playlists.OrderBy(t => t.User.UserName);
                }
            }
            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await playlists.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }
        public async Task<Playlist> GetByIdAsync(int id)
        {
            return await _context.Playlist.Include(a => a.User).Include(a => a.Videos).FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<Playlist> CreateAsync(Playlist playlistModel)
        {
            await _context.Playlist.AddAsync(playlistModel);
            await _context.SaveChangesAsync();
            return playlistModel;
        }
        public async Task<Playlist?> UpdateAsync(int id, CreatePlaylistRequestDto playlistDto, string userId, IWebHostEnvironment _webHostEnvironment)
        {
            var existingPlaylist = await _context.Playlist.Include(a => a.User).FirstOrDefaultAsync(x => x.Id == id);

            if (userId != existingPlaylist.UserId)
            {
                throw new Exception("You have no rights to edit this post");
            }
            if (existingPlaylist == null)
            {
                return null;
            }

            existingPlaylist.Title = playlistDto.Title;
            existingPlaylist.Description = playlistDto.Description;
            existingPlaylist.Image = FileManagement.UploadImage(playlistDto.Image, _webHostEnvironment);


            await _context.SaveChangesAsync();

            return existingPlaylist;
        }
        public async Task<Playlist> DeleteByIdAsync(int id, string userId)
        {
            var playlistModel = await _context.Playlist.FindAsync(id);

            if (playlistModel == null)
            {
                return null;
            }

            if (userId != playlistModel.UserId)
            {
                throw new Exception("You have no rights to delete this post");
            }
            _context.Playlist.Remove(playlistModel);
            await _context.SaveChangesAsync();
            return playlistModel;
        }
    }
}
