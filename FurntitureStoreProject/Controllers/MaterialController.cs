using FurntitureStoreProject.Data;
using FurntitureStoreProject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FurntitureStoreProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        [HttpGet("GetAllCompatibleMaterials/{FurnitureBaseId}")]
        public ActionResult<List<Material>> GetAllCompatibleMaterials(int FurnitureBaseId)
        {
            return Ok(MaterialData.GetAllCompatibleMaterials(FurnitureBaseId));
        }

        [HttpGet("GetMaterialItem/{MaterialId}")]
        public ActionResult<Material> GetMaterialItem(int MaterialId)
        {
            return Ok(MaterialData.GetMaterialItem(MaterialId));
        }

        [HttpPost("AddMaterials")]
        public ActionResult<bool> AddMaterials(Material MaterialItem)
        {
            return Ok(MaterialData.AddMaterials(MaterialItem));
        }

        [HttpPut("UpdateMaterial")]
        public ActionResult<bool> UpdateMaterial(Material MaterialItem)
        {
            return Ok(MaterialData.UpdateMaterial(MaterialItem));
        }

        [HttpGet("GetColourMaterialId/{MaterialId}/{ColourId}")]
        public ActionResult<int> GetColourMaterialId(int MaterialId, int ColourId)
        {
            return Ok(MaterialData.GetColourMaterialId(MaterialId, ColourId));
        }
        [HttpPost("AddColourMaterial/{MaterialId}/{ColourId}")]
        public ActionResult AddColourMaterial(int MaterialId, int ColourId)
        {
            return Ok(MaterialData.AddColourMaterial(ColourId, MaterialId));
        }


    }
}
