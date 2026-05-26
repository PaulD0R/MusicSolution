using CommentService.API.Extensions;
using CommentService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommentService.API.Controllers;

[ApiController]
[Authorize]
[Route("comments")]
public class CommentController(
    ICommentService commentService,
    ILogger<CommentController> logger)
    : ControllerBase
{
    [HttpGet("musics/{musicId:guid}")]
    public async Task<IActionResult> GetByMusicId([FromRoute]Guid musicId)
    {
        logger.LogInformation("GET comments/musics/{MusicId}", musicId);
        return Ok(await commentService.GetCommentsByMusicIdAsync(musicId));
    }
    
    [HttpGet("{parentId:guid}/responses")]
    public async Task<IActionResult> GetByParentId([FromRoute]Guid parentId)
    {
        logger.LogInformation("GET comments/{ParentId}/response", parentId);
        return Ok(await commentService.GetCommentsByParentIdAsync(parentId));
    }

    [HttpGet("{id:guid}/video")]
    [RequestSizeLimit(52_428_800)]
    public async Task<IActionResult> GetVideoById([FromRoute]Guid id)
    {
        logger.LogInformation("GET comments/{id}/video", id);
        return File(await commentService.GetFileByIdAsync(id), "video/mp4", enableRangeProcessing: true);
    } 

    [HttpPost("musics/{musicId:guid}")]
    public async Task<IActionResult> Add([FromRoute] Guid musicId, IFormFile file)
    {
        logger.LogInformation("POST comments/musics/{musicId}", musicId);
        var userId = User.GetUserId();
        if (userId == null) return Unauthorized();
        await commentService.AddAsync(file, userId, musicId);
        logger.LogInformation("POST successfully");
        return Created();
    }
    
    [HttpPost("{parentId:guid}/responses")]
    public async Task<IActionResult> AddResponse([FromRoute] Guid parentId, IFormFile file)
    {
        logger.LogInformation("POST {parentId}/response", parentId);
        var userId = User.GetUserId();
        if (userId == null) return Unauthorized();
        await commentService.AddAsync(file, userId, null, parentId);
        logger.LogInformation("POST successfully");
        return Created();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute]Guid id)
    {
        logger.LogInformation("TCP DELETE /comments/{Guid}", id);
        await commentService.DeleteAsync(id);
        logger.LogInformation("DELETE successfully");
        return NoContent();
    }
}