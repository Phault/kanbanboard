using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Kanbanboard.Auth;
using Kanbanboard.Data;
using Kanbanboard.Model;
using Kanbanboard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Kanbanboard.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/")]
    public class ProfileController : ControllerBase
    {
        private readonly AuthorizedRepository _repository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public ProfileController(AuthorizedRepository repository, UserManager<AppUser> userManager, IMapper mapper)
        {
            _repository = repository;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ProfileViewModel>> GetProfile()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(id);
            var profile = _mapper.Map<ProfileViewModel>(user);
            return profile;
        }

        [HttpGet("boards")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<BoardViewModel>>> GetBoards()
        {
            var domainBoards = await _repository.GetBoardsOwnedAsync();
            var dtoBoards = _mapper.Map<IEnumerable<BoardViewModel>>(domainBoards);
            return Ok(dtoBoards);
        }
    }
}