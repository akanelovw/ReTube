using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReTube.Data;
using ReTube.Models;
using ReTube.Dtos.Playlist;
using ReTube.Mappers;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ReTube.Interfaces;
using ReTube.Repository;
using ReTube.Dtos.Video;
using ReTube.Helpers;
using ReTube.Service;

namespace ReTube.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PlaylistController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IPlaylistRepository playlistRepository, IWebHostEnvironment webHostEnvironment)
        {
            _playlistRepository = playlistRepository;
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/Playlist
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var playlists = await _playlistRepository.GetAllAsync(query);

            var playlistsDto = playlists.Select(s => s.ToPlaylistDto()).ToList();

            return Ok(playlistsDto);
        }

        // GET: api/Videos/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var playlist = await _playlistRepository.GetByIdAsync(id);

            if (playlist == null)
            {
                return NotFound();
            }

            return Ok(playlist.ToPlaylistDto());
        }

        // POST: api/Playlist
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreatePlaylistRequestDto playlistDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userName = new string(User.FindFirstValue(ClaimTypes.GivenName));
            var appUser = await _userManager.FindByNameAsync(userName);

            if (appUser == null)
                return NotFound("You need to login first");

            var playlistModel = playlistDto.ToPlaylistFromCreateDto();
            playlistModel.UserId = appUser.Id;
            playlistModel.Image = FileManagement.UploadImage(playlistDto.Image, _webHostEnvironment);
            await _playlistRepository.CreateAsync(playlistModel);
            return CreatedAtAction(nameof(GetById), new { id = playlistModel.Id }, playlistModel.ToPlaylistCreateDto());
        }

        // PUT: api/Playlist/5
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromForm] CreatePlaylistRequestDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userName = new string(User.FindFirstValue(ClaimTypes.GivenName));
            var appUser = await _userManager.FindByNameAsync(userName);

            if (appUser == null)
                return NotFound("You need to login first");

            try
            {
                var playlistModel = await _playlistRepository.UpdateAsync(id, updateDto, appUser.Id, _webHostEnvironment);
                if (playlistModel == null)
                {
                    return NotFound();
                }
                return Ok(playlistModel.ToPlaylistCreateDto());
            }
            catch (Exception ex)
            {
                {
                    return Problem(
                        type: "/docs/errors/forbidden",
                        title: $"{ex.Message}",
                        detail: $"User '{appUser}' doesn't have right to edit this post.",
                        statusCode: StatusCodes.Status403Forbidden,
                        instance: HttpContext.Request.Path
                    );
                }
            }
        }

        // DELETE: api/Playlist/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var userName = new string(User.FindFirstValue(ClaimTypes.GivenName));
            var appUser = await _userManager.FindByNameAsync(userName);

            if (appUser == null)
                return NotFound("You need to login first");

            try
            {
                var playlist = await _playlistRepository.DeleteByIdAsync(id, appUser.Id);
                if (playlist == null)
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
    }
}