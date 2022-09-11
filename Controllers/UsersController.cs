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

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var usersToReturn = await _userRepository.GetMembersAync();
            return Ok(usersToReturn);
        }

        [HttpGet(template: "{username}")] //api/users/3
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            MemberDto user = await _userRepository.GetMemberAync(username);

            if(user != null)
            {
                return Ok(user);
            }

            return NotFound();
        }
    }
}