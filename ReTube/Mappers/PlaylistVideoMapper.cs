using ReTube.Dtos.PlaylistVideo;
using ReTube.Models;

namespace ReTube.Mappers
{
    public static class PlaylistVideoMapper
    {
        public static PlaylistVideo ToPlaylistVideoFromCreateDto(this CreatePlaylistVideoRequestDto playlistVideoDto)
        {
            return new PlaylistVideo
            {
                VideoId = playlistVideoDto.VideoId,
                PlaylistId = playlistVideoDto.PlaylistId
            };
        }
    }
}
