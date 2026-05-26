using System.Security.Cryptography;
using RoomService.Application.DTOs.Musics;
using RoomService.Application.DTOs.Rooms;
using RoomService.Application.Interfaces.Repositories;
using RoomService.Application.Interfaces.Services;
using RoomService.Application.Mappers;
using RoomService.Domain.Exceptions;
using RoomService.Domain.Models;

namespace RoomService.Application.Services;

public class RoomService(
    IRoomRepository roomRepository,
    IPersonService personService) : IRoomService
{
    private const int SaltSize = 8;
    private const int KeySize = 16;
    private const int Iterations = 100000;
    private readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA256;
    private const char Separator = ':';

    public async Task<RoomDto> AddRoomAsync(NewRoomRequest roomRequest, string personId)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes
            .Pbkdf2(roomRequest.Password, salt, Iterations, _hashAlgorithm, KeySize);

        var saltBase64 = Convert.ToBase64String(salt);
        var hashBase64 = Convert.ToBase64String(hash);
        var passwordField = $"{saltBase64}{Separator}{hashBase64}"; 

        var newRoom = new Room
        {
            Id = GenerateId(),
            AdminId = personId,
            Password = passwordField,
            Persons = [await personService.GetPersonByIdAsync(personId)]
        };

        var room = await roomRepository.AddRoomAsync(newRoom);
        return room != null ? room.ToRoomDto(await personService.GetPersonNamesByRoomIdAsync(room.Id)) 
            : throw new InternalServerException("Failed to add room");
    }

    public async Task JoinToRoomAsync(JoinRoomRequest joinRequest, string personId)
    {
        var password = await roomRepository.GetPasswordAsync(joinRequest.RoomId);
        if (password == null) throw new NotFoundException("Room not found");
        var passwordParts = password.Split(Separator);
        if (passwordParts.Length != 2) throw new InternalServerException("Invalid password");
        
        var salt = Convert.FromBase64String(passwordParts[0]);
        var originalHash = Convert.FromBase64String(passwordParts[1]);

        var newHash = Rfc2898DeriveBytes
            .Pbkdf2(joinRequest.Password, salt, Iterations, _hashAlgorithm, KeySize);

        if (!CryptographicOperations.FixedTimeEquals(originalHash, newHash))
            throw new BadRequestException("Invalid password");

        var person = await personService.GetPersonByIdAsync(personId);
        await roomRepository.AddPersonToRoomAsync(joinRequest.RoomId, person);
    }

    public async Task<MusicDto> ChangeMusicDataAsync(MusicActionRequest musicActionRequest, string personId)
    {
        var room = await roomRepository.GetRoomByIdAsync(musicActionRequest.RoomId);
        if (room == null) throw new Exception("Room not found");
        if (!await roomRepository.IsUserAsync(room.Id, personId)) 
            throw new ForbiddenException("Insufficient rights");

        room.MusicId = musicActionRequest.MusicId;  
        room.IsActive = musicActionRequest.IsActive;
        room.ActionTime = DateTime.UtcNow;
        room.Position = musicActionRequest.NewPosition;
        
        await roomRepository.UpdateRoomAsync(room);

        return room.ToMusicDto();
    }

    public async Task RemoveAsync(string roomId, string personId)
    {
        if (await roomRepository.GetRoomByIdAsync(roomId) == null)
            throw new NotFoundException("Room not found");
        if (!await roomRepository.IsAdminAsync(roomId, personId))
            throw new ForbiddenException("Insufficient rights");
        
        await roomRepository.RemoveRoomAsync(roomId);
    }

    public async Task<RoomDto> GetRoomAsync(string roomId, string personId)
    {
        var room = await roomRepository.GetRoomByIdAsync(roomId) ?? throw new NotFoundException("Room not found");
        if (!await roomRepository.IsUserAsync(roomId, personId))
            throw new ForbiddenException("Insufficient rights");

        return room.ToRoomDto(await personService.GetPersonNamesByRoomIdAsync(room.Id));
    }

    public async Task<IEnumerable<string>> GetRoomIdsByPersonIdAsync(string personId)
    {
        return await roomRepository.GetRoomIdsByPersonIdAsync(personId);
    }

    private string GenerateId()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 5)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}