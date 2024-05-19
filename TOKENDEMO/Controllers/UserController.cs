using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TOKENDEMO.Models;
using TOKENDEMO.Repository;

namespace TOKENDEMO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> Create(User user)
        {
            var userCreated = await _userRepository.AddAsync(user);
            return Created("success", userCreated);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(int id,User user)
        {
       
            var userExisted = await _userRepository.GetByIdAsync(id);
            if (userExisted != null)
            {
                if (id != user.Id)
                {
                    return BadRequest();
                }
                var userUpdated = await _userRepository.UpdateAsync(user);
                return Ok(userUpdated);
            }

            else {
                return NotFound();
            }
        
        }
    }
}
