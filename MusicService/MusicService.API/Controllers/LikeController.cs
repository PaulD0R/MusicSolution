using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicService.API.Extensions;
using MusicService.Application.Interfaces.Services;

namespace MusicService.API.Controllers;

[ApiController]
[Authorize]
[Route("likes")]
public class LikeController(
    ILikeService likeService,
    ILogger<LikeController> logger)
    : ControllerBase
{
    [HttpPost("{musicId:guid}")]
    public async Task<IActionResult> Add([FromRoute] Guid musicId)
    {
        logger.LogInformation("GET likes/{MusicID}", musicId);
        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        await likeService.AddAsync(musicId, userId);
        return Created();
    }
    
    [HttpDelete("{musicId:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid musicId)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        await likeService.DeleteAsync(musicId, userId);
        return NoContent();
    }

    [HttpGet("person")]
    public async Task<IActionResult> GetPerson()
    {
        logger.LogInformation("GET likes/person");
        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();
        
        return Ok(await likeService.GetLikeMusicByUserIdAsync(userId));
    }
}