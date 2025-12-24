using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReTube.Data;
using ReTube.Dtos.PlaylistVideo;
using ReTube.Interfaces;
using ReTube.Models;
using ReTube.Mappers;
using ReTube.Repository;
using System.Security.Claims;

namespace ReTube.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistVideoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPlaylistVideoRepository _playlistVideoRepository;

        public PlaylistVideoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IPlaylistVideoRepository playlistVideoRepository)
        {
            _playlistVideoRepository = playlistVideoRepository;
            _context = context;
            _userManager = userManager;
        }

        // POST: api/Videos
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePlaylistVideoRequestDto playlistVideoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userName = new string(User.FindFirstValue(ClaimTypes.GivenName));
            var appUser = await _userManager.FindByNameAsync(userName);

            if (appUser == null)
                return NotFound("You need to login first");

            var playlistVideoModel = playlistVideoDto.ToPlaylistVideoFromCreateDto();
            var playlist = await _context.Playlist.SingleOrDefaultAsync(a => a.Id == playlistVideoModel.PlaylistId);
            if (playlist.UserId != appUser.Id)
                return BadRequest();
            if (PlaylistVideoExists(playlistVideoModel.VideoId, playlistVideoModel.PlaylistId))
                return BadRequest();
            await _playlistVideoRepository.CreateAsync(playlistVideoModel);
            return Ok(playlistVideoModel);
        }
        // DELETE: api/Videos/
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync([FromBody] DeletePlaylistVideoRequestDto playlistVideoDto)
        {
            var userName = new string(User.FindFirstValue(ClaimTypes.GivenName));
            var appUser = await _userManager.FindByNameAsync(userName);

            if (appUser == null)
                return NotFound("You need to login first");

            try
            {
                var song = await _playlistVideoRepository.DeleteAsync(playlistVideoDto.VideoId, playlistVideoDto.PlaylistId, appUser.Id);
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
        private bool PlaylistVideoExists(int videoId, int playlistId)
        {
            return _context.PlaylistVideo.Any(a => a.VideoId == videoId && a.PlaylistId == playlistId);
        }
    }

}
