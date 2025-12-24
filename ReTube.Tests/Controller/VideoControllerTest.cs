using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using ReTube.Interfaces;
using ReTube.Dtos.Video;
using ReTube.Models;
using ReTube.Controllers;
using ReTube.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using ReTube.Mappers;
using Moq;
using System.Security.Claims;
using System.Security.Principal;
using Azure;
using ReTube.Service;

namespace ReTube.Tests.Controller
{
    public class VideoControllerTest
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IVideoRepository _videoRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VideoControllerTest() 
        {
            _videoRepository = A.Fake<IVideoRepository>();
            _userManager = A.Fake<UserManager<ApplicationUser>>();
        }
        [Fact]
        public async Task VideoController_GetAll_ReturnOk()
        {
            // Arrange
            var videoList = A.Fake<List<Video>>();
            var query = A.Fake<QueryObject>();
            A.CallTo(() => _videoRepository.GetAllAsync(query)).Returns(GetTestVideos());
            var controller = new VideoController(_userManager, _videoRepository);

            // Act
            var result = await controller.GetAll(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IList<VideoDto>>(okResult.Value);
        }

        [Fact]
        public async Task VideoController_GetById_ReturnOk()
        {
            // Arrange
            var videoList = A.Fake<List<Video>>();
            var query = A.Fake<QueryObject>();
            A.CallTo(() => _videoRepository.GetByIdAsync(1)).Returns(GetTestVideo());
            var controller = new VideoController(_userManager, _videoRepository);

            // Act
            var result = await controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<VideoDto>(okResult.Value);
        }

        [Fact]
        public async Task VideoController_Create_ReturnGetById()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "example name"),
                    new Claim(ClaimTypes.GivenName, "example name"),
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim("custom-claim", "example claim value"),
                }, "mock"));

            var videoDto = GetTestVideoDto();
            var videoModel = A.Fake<Video>();
            var testUser = GetTestUser();
            A.CallTo(() => _userManager.FindByNameAsync("example name")).Returns(testUser);
            A.CallTo(() => _videoRepository.CreateAsync(videoDto, testUser.Id)).Returns(GetTestVideo());
            var controller = new VideoController(_userManager, _videoRepository);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            // Act
            var result = await controller.Create(videoDto);

            // Assert
            var okResult = Assert.IsType<CreatedAtActionResult>(result);
            A.CallTo(() => _videoRepository.CreateAsync(videoDto, testUser.Id)).MustHaveHappened();
        }

        [Fact]
        public async Task VideoController_Update_ReturnOk()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "example name"),
                    new Claim(ClaimTypes.GivenName, "example name"),
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim("custom-claim", "example claim value"),
                }, "mock"));
            var videoDto = GetTestUpdateVideoDto();
            var videoModel = A.Fake<Video>();
            var testUser = GetTestUser();
            A.CallTo(() => _userManager.FindByNameAsync("example name")).Returns(testUser);
            A.CallTo(() => _videoRepository.UpdateAsync(1, videoDto, testUser.Id)).Returns(GetTestVideo());
            var controller = new VideoController(_userManager, _videoRepository);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            // Act
            var result = await controller.Update(1, videoDto);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task VideoController_Update_ReturnException()
        {
            // Arrange
            var user = GetClaimsSecondUser();
            var videoDto = GetTestUpdateVideoDto();
            var videoModel = A.Fake<Video>();
            var testUser = GetSecondTestUser();
            A.CallTo(() => _userManager.FindByNameAsync("example name2")).Returns(testUser);
            A.CallTo(() => _videoRepository.UpdateAsync(1, videoDto, testUser.Id)).ThrowsAsync(new Exception("You have no rights to edit this post"));
            var controller = new VideoController(_userManager, _videoRepository);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            // Act
            var result = await controller.Update(1, videoDto);
            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public async Task VideoController_Delete_NoContentResult()
        {
            // Arrange
            var user = GetClaims();
            var testUser = GetTestUser();
            A.CallTo(() => _userManager.FindByNameAsync("example name")).Returns(testUser);
            A.CallTo(() => _videoRepository.DeleteByIdAsync(1, testUser.Id)).Returns(GetTestVideo());
            var controller = new VideoController(_userManager, _videoRepository);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            // Act
            var result = await controller.DeleteById(1);
            // Assert
            var okResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task VideoController_Delete_NotFoundResult()
        {
            // Arrange
            var user = GetClaims();
            var testUser = GetTestUser();
            Video returnValue = null;
            A.CallTo(() => _userManager.FindByNameAsync("example name")).Returns(testUser);
            A.CallTo(() => _videoRepository.DeleteByIdAsync(1, testUser.Id)).Returns(returnValue);
            var controller = new VideoController(_userManager, _videoRepository);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            // Act
            var result = await controller.DeleteById(1);
            // Assert
            var okResult = Assert.IsType<NotFoundResult>(result);
        }

        private List<Video> GetTestVideos()
        {
            var user = A.Fake<ApplicationUser>();
            var videos = new List<Video>
            {
                new Video { Title="Cock1", Description="asdas1241fasf", Image="asdasd.jpg", User = user },
                new Video { Title="Cock2", Description="asdas124fasf", Image="asda12344sd.jpg", User = user },
                new Video { Title="Cock3", Description="asdasas2123fasf", Image="asdas12asd.jpg", User = user },
                new Video { Title="Cock4", Description="asdas12412fasf", Image="asdasa1213sd.jpg", User = user },
            };
            return videos;
        }
        private Video GetTestVideo()
        {
            var user = GetTestUser();
            return new Video { Title = "Cock1", Description = "asdas1241fasf", Image = "asdasd.jpg", User = user };
        }
        private CreateVideoRequestDto GetTestVideoDto()
        {
            var filebytes = Encoding.UTF8.GetBytes("dummy image");
            IFormFile file = new FormFile(new MemoryStream(filebytes), 0, filebytes.Length, "Data", "image.png");
            return new CreateVideoRequestDto { Title = "Cock1", Description = "asdas1241fasf", Image = file };
        }
        private UpdateVideoRequestDto GetTestUpdateVideoDto()
        {
            var filebytes = Encoding.UTF8.GetBytes("dummy image");
            IFormFile file = new FormFile(new MemoryStream(filebytes), 0, filebytes.Length, "Data", "image.png");
            return new UpdateVideoRequestDto { Title = "Cock1", Description = "asdas1241fasf", Image = file };
        }
        private ApplicationUser GetTestUser()
        {
            return new ApplicationUser { Id = "1231253sasdrfasf", UserName = "Test", Email = "test@test.com" };
        }

        private ClaimsPrincipal GetClaims()
        {
            var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "example name"),
                    new Claim(ClaimTypes.GivenName, "example name"),
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim("custom-claim", "example claim value"),
                }, "mock"));
            return claims;
        }
        private ClaimsPrincipal GetClaimsSecondUser()
        {
            var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "example name2"),
                    new Claim(ClaimTypes.GivenName, "example name2"),
                    new Claim(ClaimTypes.NameIdentifier, "2"),
                    new Claim("custom-claim", "example claim value"),
                }, "mock"));
            return claims;
        }
        private ApplicationUser GetSecondTestUser()
        {
            return new ApplicationUser { Id = "1231253sasasfasfdrfasf", UserName = "Test2", Email = "test2@test.com" };
        }

    }
}
