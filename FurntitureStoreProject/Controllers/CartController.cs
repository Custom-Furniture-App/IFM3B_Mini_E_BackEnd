using FurntitureStoreProject.Data;
using FurntitureStoreProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace FurntitureStoreProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        [HttpGet("GetActiveCart/{UserId}")]
        public ActionResult<Cart> GetActiveCart(int UserId)
        {
            return Ok(CartData.GetActiveCart(UserId));
        }

        [HttpPost("CreateCart/{UserId}")]
        public ActionResult<bool> CreateCart(int UserId)
        {
            return Ok(CartData.CreateCart(UserId));
        }

        [HttpGet("GetCart/{CartId}")]
        public ActionResult<Cart> GetCart(int CartId)
        {
            return Ok(CartData.GetCart(CartId));
        }

        [HttpGet("GetAllCarts/{UserId}")]
        public ActionResult<List<Cart>> GetAllCarts(int UserId)
        {
            return Ok(CartData.GetAllCarts(UserId));
        }
    }
}
