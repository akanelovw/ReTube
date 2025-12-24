
using Microsoft.AspNetCore.Mvc;
using ReTube.Service;
using ReTube.Data;
using ReTube.Models;
using ReTube.Dtos.Video;
using ReTube.Mappers;

using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ReTube.Interfaces;

using ReTube.Helpers;
using Microsoft.AspNetCore.Hosting;


namespace ReTube.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IVideoRepository _videoRepository;

        public VideoController(UserManager<ApplicationUser> userManager, IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
            _userManager = userManager;
        }

        // GET: api/Videos
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var videos = await _videoRepository.GetAllAsync(query);

            var videosDto = videos.Select(s => s.ToVideoDto()).ToList();

            return Ok(videosDto);
        }

        // GET: api/Videos/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var video = await _videoRepository.GetByIdAsync(id);

            if (video == null)
            {
                return NotFound();
            }

            return Ok(video.ToVideoDto());
        }

        // PUT: api/Videos/5
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromForm] UpdateVideoRequestDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userName = new string(User.FindFirstValue(ClaimTypes.GivenName));
            var appUser = await _userManager.FindByNameAsync(userName);
            if (appUser == null)
                return NotFound($"You need to login first");
            try
            {
                var videoModel = await _videoRepository.UpdateAsync(id, updateDto, appUser.Id);
                if (videoModel == null)
                {
                    return NotFound();
                }
                return Ok(videoModel.ToVideoDto());
            }
            catch (Exception ex)
            {
                {
                    return Problem(
                        type: "/docs/errors/forbidden",
                        title: $"{ex.Message}",
                        detail: $"User '{userName}' doesn't have right to delete this post.",
                        statusCode: StatusCodes.Status403Forbidden,
                        instance: HttpContext.Request.Path
                    );
                }
            }
        }

        // POST: api/Videos
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateVideoRequestDto videoDto) 
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userName = new string(User.FindFirstValue(ClaimTypes.GivenName));
            var appUser = await _userManager.FindByNameAsync(userName);

            if (appUser == null)
                return NotFound($"You need to login first");

            
            var video = await _videoRepository.CreateAsync(videoDto, appUser.Id);
            return CreatedAtAction(nameof(GetById), new { id = video.Id }, video.ToVideoDto());
        }

        // DELETE: api/Videos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var userName = new string(User.FindFirstValue(ClaimTypes.GivenName));
            var appUser = await _userManager.FindByNameAsync(userName);

            if (appUser == null)
                return NotFound($"You need to login first");

            try
            {
                var video = await _videoRepository.DeleteByIdAsync(id, appUser.Id);
                if (video == null)
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
                    detail: $"User '{userName}' doesn't have right to delete this post.",
                    statusCode: StatusCodes.Status403Forbidden,
                    instance: HttpContext.Request.Path
                );
            }
        }

        private async Task<bool> VideoExists(int id)
        {
            return await _videoRepository.GetAnyAsync(id);
        }
    }
}