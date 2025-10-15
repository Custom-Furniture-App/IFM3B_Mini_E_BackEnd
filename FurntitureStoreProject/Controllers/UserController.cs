using FurntitureStoreProject.Data;
using FurntitureStoreProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FurntitureStoreProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("Login/{UserEmail}/{UserPassword}")]
        public ActionResult<User> Login(string UserEmail, string UserPassword)
        {
            User UserLogin = UserData.Login(UserEmail, UserPassword);
            if (UserLogin == null)
            {
                return BadRequest(UserLogin);
            }
            else
            {
                return Ok(UserLogin);
            }
        }
    
        [HttpGet("IsAdmin/{UserId}")]
        public ActionResult<bool> IsAdmin(int UserId)
        {
            return Ok(UserData.IsAdmin(UserId));
        }

        [HttpPost("Register/{Password}")]
        public ActionResult<bool> Register(User NewUser, string Password)
        {
            return Ok(UserData.Register(NewUser, Password));
        }
        [HttpPut("EditUser/{Password}")]
        public ActionResult<bool> EditUser(User EditUser, string Password)
        {
            return Ok(UserData.EditUser(EditUser, Password));
        }

    }
}
