using FurntitureStoreProject.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FurntitureStoreProject.Model;

namespace FurntitureStoreProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentController : ControllerBase
    {

        [HttpGet("GetAllCompatibleComponents/{FurnitureBaseId}")]
        public ActionResult<List<Component>> GetAllCompatibleComponents(int FurnitureBaseId)
        {
            return Ok(ComponentData.GetAllCompatibleComponents(FurnitureBaseId));
        }

        [HttpGet("GetComponentItem/{ComponentId}")]
        public ActionResult<Component> GetComponentItem(int ComponentId)
        {
            return Ok(ComponentData.GetComponentItem(ComponentId));
        }

        [HttpPost("AddComponent")]
        public ActionResult<bool> AddComponent(Component ComponentItem)
        {
            return Ok(ComponentData.AddComponent(ComponentItem));
        }

        [HttpPut("UpdateComponent")]
        public ActionResult<bool> UpdateComponent(Component ComponentItem)
        {
            return Ok(ComponentData.UpdateComponent(ComponentItem));
        }

        [HttpPost("AddComponentToFurnitureBase/{FurnitureBaseId}/{ComponentId}")]
        public ActionResult<bool> AddComponentToFurnitureBase(int FurnitureBaseId, int ComponentId)
        {
            return Ok(ComponentData.AddComponentToFurnitureBase(FurnitureBaseId, ComponentId));
        }
    }
}
