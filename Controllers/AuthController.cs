using System.Threading.Tasks;
using Kanbanboard.Auth;
using Kanbanboard.Model;
using Kanbanboard.ViewModels.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Kanbanboard.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]/")]
    public class AuthController : ControllerBase
    {
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;
        private readonly JwtFactory _jwtFactory;

        public AuthController(UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager,
            JwtFactory jwtFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtFactory = jwtFactory;
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([FromBody] RegistrationViewModel model)
        {
            var user = new AppUser { 
                UserName = model.Username,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                // var callbackUrl = Url.Page("Account/ConfirmEmail",
                //     pageHandler: null,
                //     values: new {userId = user.Id, code = code},
                //     protocol: Request.Scheme);

                // await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                //     $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                
                var token = _jwtFactory.CreateToken(user);
                return Ok(token);
            }

            return BadRequest(result.Errors);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Login([FromBody] CredentialsViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
                return BadRequest();

            var result = await _signInManager.CheckPasswordSignInAsync(
                user, 
                model.Password,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var token = _jwtFactory.CreateToken(user);
                return Ok(token);
            }
            
            if (result.IsLockedOut)
                return BadRequest("You are locked out, try again later.");

            return BadRequest();
        }
    }
}