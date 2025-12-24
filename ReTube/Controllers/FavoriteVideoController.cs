using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReTube.Data;
using ReTube.Dtos.FavoriteVideo;
using ReTube.Mappers;
using ReTube.Interfaces;
using ReTube.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace ReTube.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteVideoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFavoriteVideoRepository _favoriteVideoRepository;

        public FavoriteVideoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IFavoriteVideoRepository favoriteVideoRepository)
        {
            _favoriteVideoRepository = favoriteVideoRepository;
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Videos
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var videos = await _favoriteVideoRepository.GetAllAsync();

            var videosDto = videos.Select(s => s.ToFavoriteVideoDto()).ToList();

            return Ok(videosDto);
        }

        // GET: api/Videos/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var favoriteVideo = await _favoriteVideoRepository.GetByIdAsync(id);

            if (favoriteVideo == null)
            {
                return NotFound();
            }

            return Ok(favoriteVideo.ToFavoriteVideoDto());
        }

        // POST: api/Videos
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFavoriteVideoRequestDto favoriteVideoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userName = new string(User.FindFirstValue(ClaimTypes.GivenName));
            var appUser = await _userManager.FindByNameAsync(userName);

            if (appUser == null)
                return NotFound("You need to login first");

            var favorite = await _context.FavoriteVideo.SingleOrDefaultAsync(a => a.UserId == appUser.Id);

            var videoModel = favoriteVideoDto.ToFavoriteVideoFromCreateDto();
            videoModel.FavoriteVideoId = favorite.Id;
            if (FavoriteVideoToVideoExists(videoModel.VideoId, favorite.Id))
                return BadRequest();
            await _favoriteVideoRepository.CreateAsync(videoModel);
            return Ok(videoModel);
        }

        // DELETE: api/Videos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var userName = new string(User.FindFirstValue(ClaimTypes.GivenName));
            var appUser = await _userManager.FindByNameAsync(userName);

            if (appUser == null)
                return NotFound("You need to login first");

            try
            {
                var song = await _favoriteVideoRepository.DeleteByIdAsync(id, appUser.Id);
                if (song == null)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(
                    type: "/docs/errors/forbidden",
                    title: "Authenticated user is not authorized.",
                    detail: $"User '{appUser}' doesn't have right to delete this post.",
                    statusCode: StatusCodes.Status403Forbidden,
                    instance: HttpContext.Request.Path
                );
            }
        }

        private bool FavoriteVideoToVideoExists(int videoId, int favoriteVideoId)
        {
            return _context.FavoriteVideoToVideo.Any(a => a.VideoId == videoId && a.FavoriteVideoId == favoriteVideoId);
        }
    }
}
