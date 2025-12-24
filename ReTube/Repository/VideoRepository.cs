using Microsoft.EntityFrameworkCore;
using ReTube.Data;
using ReTube.Interfaces;
using ReTube.Models;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ReTube.Dtos.Video;
using ReTube.Helpers;
using Microsoft.AspNetCore.Hosting;
using ReTube.Service;
using ReTube.Mappers;

namespace ReTube.Repository
{
    public class VideoRepository : IVideoRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VideoRepository(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<List<Video>> GetAllAsync(QueryObject query)
        {
            var videos = _context.Video.Include(a => a.User).AsQueryable();
            if (!string.IsNullOrWhiteSpace(query.Title))
            {
                videos = videos.Where(v => v.Title.Contains(query.Title));
            }
            if (!string.IsNullOrWhiteSpace(query.Username))
            {
                videos = videos.Where(v => v.User.UserName.Contains(query.Username));
            }
            if (string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Title", StringComparison.OrdinalIgnoreCase))
                {
                    videos = query.IsDecsending ? videos.OrderByDescending(t => t.Title) : videos.OrderBy(t => t.Title);
                }
                if (query.SortBy.Equals("Username", StringComparison.OrdinalIgnoreCase))
                {
                    videos = query.IsDecsending ? videos.OrderByDescending(t => t.User.UserName) : videos.OrderBy(t => t.User.UserName);
                }
            }
            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await videos.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }
        public async Task<Video> GetByIdAsync(int id)
        {
            return await _context.Video.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<Video> CreateAsync(CreateVideoRequestDto videoDto, string userName)
        {
            var videoModel = videoDto.ToVideoFromCreateDto();
            videoModel.UserId = userName;
            videoModel.Image = FileManagement.UploadImage(videoDto.Image, _webHostEnvironment);
            await _context.Video.AddAsync(videoModel);
            await _context.SaveChangesAsync();
            return videoModel;
        }


        public async Task<Video?> UpdateAsync(int id, UpdateVideoRequestDto videoDto, string userId)
        {
            var existingVideo = await _context.Video.Include(a => a.User).FirstOrDefaultAsync(x => x.Id == id);

            if (userId != existingVideo.UserId)
            {
                throw new Exception("You have no rights to edit this post");
            }
            if (existingVideo == null)
            {
                return null;
            }

            existingVideo.Title = videoDto.Title;
            existingVideo.Description = videoDto.Description;
            existingVideo.Image = FileManagement.UploadImage(videoDto.Image, _webHostEnvironment);
            existingVideo.VideoFile = videoDto.VideoFile;


            await _context.SaveChangesAsync();

            return existingVideo;
        }
        public async Task<Video> DeleteByIdAsync(int id, string userId)
        {
            var videoModel = await _context.Video.FindAsync(id);

            if (videoModel == null)
            {
                return null;
            }

            if (userId != videoModel.UserId)
            {
                throw new Exception("You have no rights to delete this post");
            }
            _context.Video.Remove(videoModel);
            await _context.SaveChangesAsync();
            return videoModel;
        }
        public async Task<bool> GetAnyAsync(int id)
        {
            return await _context.Video.AnyAsync(e => e.Id == id);
        }
    }
}