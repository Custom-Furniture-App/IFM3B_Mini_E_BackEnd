using FurntitureStoreProject.Data;
using FurntitureStoreProject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FurntitureStoreProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FurnitureBaseController : ControllerBase
    {
        [HttpGet("GetAllFurnitureByType/{FurnitureTypeId}")]
        public ActionResult<List<FurnitureBase>> GetAllFurnitureByType(int FurnitureTypeId)
        {
            return Ok(FurnitureBaseData.GetAllFurnitureByType(FurnitureTypeId));
        }

        [HttpGet("GetFurnitureItem/{FurnitureBaseId}")]
        public ActionResult<FurnitureBase> GetFurnitureItem(int FurnitureBaseId)
        {
            return Ok(FurnitureBaseData.GetFurnitureItem(FurnitureBaseId));
        }

        [HttpPost("AddFurnitureBaseItem/{FurnitureTypeId}")]
        public ActionResult<bool> AddFurnitureBaseItem(FurnitureBase FurnitureBaseItem, int FurnitureTypeId)
        {
            return Ok(FurnitureBaseData.AddFurnitureBaseItem(FurnitureBaseItem, FurnitureTypeId));
        }

        [HttpPut("UpdateFurniture")]
        public ActionResult<bool> UpdateFurnitureBaseItem(FurnitureBase FurnitureBaseItem)
        {
            return Ok(FurnitureBaseData.UpdateFurnitureBaseItem(FurnitureBaseItem));
        }
    }
}
