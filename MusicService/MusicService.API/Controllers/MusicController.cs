using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicService.Application.DTOs.Helpers;
using MusicService.Application.Interfaces.Services;

namespace MusicService.API.Controllers;

[ApiController]
[Authorize]
[Route("musics")]
public class MusicController(
    IMusicService musicService,
    ILogger<MusicController> logger) 
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] MusicFindRequest findRequest)
    {
        logger.LogInformation("TCP GET /musics");
        var musics = await musicService.GetAllAsync(findRequest);
        return Ok(musics);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute]Guid id)
    {
        logger.LogInformation("TCP GET /musics/{Guid}", id);
        var music = await musicService.GetByIdAsync(id);
        return File(music.Stream, "audio/mpeg", enableRangeProcessing: true);
    }

    [HttpPost]
    [RequestSizeLimit(52_428_800)]
    public async Task<IActionResult> Add(IFormFile file, [FromForm] string name)
    {
        logger.LogInformation("TCP POST /musics");
        await musicService.AddAsync(file, name);
        logger.LogInformation("POST successfully");
        return Created();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        logger.LogInformation("TCP DELETE /musics/{Guid}", id);
        await musicService.DeleteAsync(id);
        logger.LogInformation("DELETE successfully");
        return NoContent();
    }
}