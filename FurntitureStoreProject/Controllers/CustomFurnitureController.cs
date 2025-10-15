using FurntitureStoreProject.Data;
using FurntitureStoreProject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FurntitureStoreProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomFurnitureController : ControllerBase
    {
        [HttpGet("GetCustomFurnitureWithCartId/{CartId}")]
        public ActionResult<List<CustomFurniture>> GetCustomFurnitureWithCartId(int CartId)
        {
            return Ok(CustomFurnitureData.GetCustomFurnituresWithCartId(CartId));
        }

        [HttpPost("AddCustomFurniture")]
        public ActionResult<bool> AddCustomFurniture(CustomFurniture CustomFurnitureItem)
        {
            return Ok(CustomFurnitureData.AddCustomFurniture(CustomFurnitureItem));
        }

        [HttpDelete("DelectCustom/{CustomFurnitureId}")]
        public ActionResult<bool> DeleteCustomFurniture(int CustomFurnitureId)
        {
            return Ok(CustomFurnitureData.DeleteCustomFurniture(CustomFurnitureId));
        }

        [HttpDelete("DelectCustomFurnitureCustomId/{CustomFurnitureId}")]
        public ActionResult<bool> DeleteCustomFurnitureCustomId(int CustomFurnitureId)
        {
            return Ok(CustomFurnitureData.DeleteCustomFurnitureDesignCustomId(CustomFurnitureId));
        }

        [HttpDelete("DelectCustomFurniture/{CustomFurnitureBridgeId}")]
        public ActionResult<bool> DeleteCustomDeisgnFurniture(int CustomFurnitureBridgeId)
        {
            return Ok(CustomFurnitureData.DeleteCustomFurnitureDesign(CustomFurnitureBridgeId));
        }

        [HttpPost("AddCustomFurnitureComponent/{ComponentId}/{CustomFurnitureId}")]
        public ActionResult<bool> AddCustomFurnitureDesign(int ComponentId, int CustomFurnitureId)
        {
            return Ok(CustomFurnitureData.AddCustomFurnitureComponent(ComponentId, CustomFurnitureId));
        }
    }
}
