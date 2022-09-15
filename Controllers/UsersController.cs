using System.Security.Claims;
using System.Collections.ObjectModel;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Extensions;
using CloudinaryDotNet.Actions;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var usersToReturn = await _userRepository.GetMembersAync();
            return Ok(usersToReturn);
        }

        [HttpGet(template: "{username}",Name = "GetUser")] //api/users/3
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            MemberDto user = await _userRepository.GetMemberAync(username);

            if(user != null)
            {
                return Ok(user);
            }

            return NotFound();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(username);

            _mapper.Map(memberUpdateDto, user);
            _userRepository.Update(user);

            if(await _userRepository.SaveAllAsync())
            {
                return NoContent();
            }

            return BadRequest("Failed updating the student");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            string username = User.GetUsername();
            AppUser user = await _userRepository.GetUserByUsernameAsync(username);

            var result = await _photoService.UpdloadPhotoAsync(file);

            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if(user.Photos.Count() == 0) photo.IsMain = true;


            user.Photos.Add(photo);

            if(await _userRepository.SaveAllAsync())
            {
                // return CreatedAtRoute("GetUser", _mapper.Map<PhotoDto>(photo));
                return CreatedAtRoute("GetUser", new {Username = user.UserName},_mapper.Map<PhotoDto>(photo));
            }

            return BadRequest();
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user =  await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photoToChange = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if(photoToChange.IsMain) return BadRequest("This photo is alredy main");

            var currentPhoto = user.Photos.FirstOrDefault(x => x.IsMain);
            if(currentPhoto != null) currentPhoto.IsMain = false;

            photoToChange.IsMain = true;

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("An error while uploading a photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photoToDelete = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if(photoToDelete == null) return NotFound();
            if(photoToDelete.IsMain) return BadRequest("You can not delete the main photo");

            if(photoToDelete.PublicId != null)
            {
                DeletionResult result = await _photoService.DeletePhotoAsync(photoToDelete.PublicId);
                if(result.Error != null) return BadRequest("Something went wrong when deleting the photo");
            }

            user.Photos.Remove(photoToDelete);
            if(await _userRepository.SaveAllAsync())
            {
                return Ok();
            }

            return BadRequest("Failed to delete the photo");
        }
    }
}