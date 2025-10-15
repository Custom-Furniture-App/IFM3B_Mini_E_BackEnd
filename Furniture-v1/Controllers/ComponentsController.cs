using Furniture_v1.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Furniture_v1.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ComponentsController : ControllerBase
  {
    private readonly DatabaseHelper _db;

    public ComponentsController(DatabaseHelper db)
    {
      _db = db;
    }

    // ✅ GET: api/components
    [HttpGet]
    public IActionResult GetAll()
    {
      // 1️⃣ Fetch all components
      var dt = _db.ExecuteQuery(@"
                SELECT Id, Name, Type, UnitPrice, Stock, ImageUrl, Category, Description
                FROM Components
                ORDER BY Name
            ");

      var components = dt.ToDictionaryList();

      // 2️⃣ Fetch all compatibilities
      var dtCompat = _db.ExecuteQuery(@"
                SELECT cc.ComponentId, c.Id AS CompatibleId, c.Name AS CompatibleName
                FROM ComponentCompatibilities cc
                JOIN Components c ON cc.CompatibleWithId = c.Id
            ");

      // Map compatibilities
      var compatibilities = new Dictionary<int, List<Dictionary<string, object>>>();
      foreach (DataRow row in dtCompat.Rows)
      {
        int compId = Convert.ToInt32(row["ComponentId"]);
        if (!compatibilities.ContainsKey(compId))
          compatibilities[compId] = new List<Dictionary<string, object>>();

        compatibilities[compId].Add(new Dictionary<string, object>
                {
                    { "Id", row["CompatibleId"] },
                    { "Name", row["CompatibleName"] }
                });
      }

      // Attach compatibilities to components
      foreach (var comp in components)
      {
        int compId = Convert.ToInt32(comp["Id"]);
        comp["CompatibleComponents"] = compatibilities.ContainsKey(compId)
            ? compatibilities[compId]
            : new List<Dictionary<string, object>>();
      }

      return Ok(components);
    }


    [HttpGet("category/{category}")]
    public IActionResult GetByCategory(string category)
    {
      if (string.IsNullOrWhiteSpace(category))
        return BadRequest(new { Message = "Category is required." });

      var dt = _db.ExecuteQuery(@"
                SELECT Id, Name, Type, UnitPrice, Stock, ImageUrl, Category, Description
                FROM Components
                WHERE Category = @Category
                ORDER BY Name
            ", new SqlParameter("@Category", category));

      var components = dt.ToDictionaryList();

      // Fetch all compatibilities (same as above)
      var dtCompat = _db.ExecuteQuery(@"
                SELECT cc.ComponentId, c.Id AS CompatibleId, c.Name AS CompatibleName
                FROM ComponentCompatibilities cc
                JOIN Components c ON cc.CompatibleWithId = c.Id
            ");

      var compatibilities = new Dictionary<int, List<Dictionary<string, object>>>();
      foreach (DataRow row in dtCompat.Rows)
      {
        int compId = Convert.ToInt32(row["ComponentId"]);
        if (!compatibilities.ContainsKey(compId))
          compatibilities[compId] = new List<Dictionary<string, object>>();

        compatibilities[compId].Add(new Dictionary<string, object>
                {
                    { "Id", row["CompatibleId"] },
                    { "Name", row["CompatibleName"] }
                });
      }

      foreach (var comp in components)
      {
        int compId = Convert.ToInt32(comp["Id"]);
        comp["CompatibleComponents"] = compatibilities.ContainsKey(compId)
            ? compatibilities[compId]
            : new List<Dictionary<string, object>>();
      }

      return Ok(components);
    }


    [HttpPost]
    public IActionResult AddComponent([FromBody] ComponentDto dto)
    {
      if (string.IsNullOrWhiteSpace(dto.Name) || dto.UnitPrice <= 0)
        return BadRequest(new { Message = "Invalid component data." });

      string sqlInsert = @"
                INSERT INTO Components (Name, Type, UnitPrice, Stock, ImageUrl, Category, Description)
                VALUES (@Name, @Type, @UnitPrice, @Stock, @ImageUrl, @Category, @Description);
                SELECT SCOPE_IDENTITY();
            ";

      int newId = Convert.ToInt32(_db.ExecuteScalar(sqlInsert,
          new SqlParameter("@Name", dto.Name),
          new SqlParameter("@Type", dto.Type ?? ""),
          new SqlParameter("@UnitPrice", dto.UnitPrice),
          new SqlParameter("@Stock", dto.Stock),
          new SqlParameter("@ImageUrl", dto.ImageUrl ?? ""),
          new SqlParameter("@Category", dto.Category ?? ""),
          new SqlParameter("@Description", dto.Description ?? "")
      ));

      if (dto.CompatibleComponentIds != null && dto.CompatibleComponentIds.Count > 0)
      {
        foreach (var compId in dto.CompatibleComponentIds)
        {
          _db.ExecuteNonQuery(
              "INSERT INTO ComponentCompatibilities (ComponentId, CompatibleWithId) VALUES (@ComponentId, @CompatibleWithId)",
              new SqlParameter("@ComponentId", newId),
              new SqlParameter("@CompatibleWithId", compId)
          );
        }
      }

      return Ok(new { Message = "Component added successfully", Id = newId });
    }

    [HttpPut("{id}")]
    public IActionResult UpdateComponent(int id, [FromBody] ComponentDto dto)
    {
      if (string.IsNullOrWhiteSpace(dto.Name) || dto.UnitPrice <= 0)
        return BadRequest(new { Message = "Invalid component data." });

      string sqlUpdate = @"
                UPDATE Components SET 
                    Name = @Name, 
                    Type = @Type, 
                    UnitPrice = @UnitPrice, 
                    Stock = @Stock, 
                    ImageUrl = @ImageUrl, 
                    Category = @Category, 
                    Description = @Description
                WHERE Id = @Id
            ";

      int rowsAffected = _db.ExecuteNonQuery(sqlUpdate,
          new SqlParameter("@Id", id),
          new SqlParameter("@Name", dto.Name),
          new SqlParameter("@Type", dto.Type ?? ""),
          new SqlParameter("@UnitPrice", dto.UnitPrice),
          new SqlParameter("@Stock", dto.Stock),
          new SqlParameter("@ImageUrl", dto.ImageUrl ?? ""),
          new SqlParameter("@Category", dto.Category ?? ""),
          new SqlParameter("@Description", dto.Description ?? "")
      );

      if (rowsAffected == 0)
        return NotFound(new { Message = $"Component with ID {id} not found." });

      _db.ExecuteNonQuery(
          "DELETE FROM ComponentCompatibilities WHERE ComponentId = @ComponentId",
          new SqlParameter("@ComponentId", id)
      );

      if (dto.CompatibleComponentIds != null && dto.CompatibleComponentIds.Count > 0)
      {
        foreach (var compId in dto.CompatibleComponentIds)
        {
          _db.ExecuteNonQuery(
              "INSERT INTO ComponentCompatibilities (ComponentId, CompatibleWithId) VALUES (@ComponentId, @CompatibleWithId)",
              new SqlParameter("@ComponentId", id),
              new SqlParameter("@CompatibleWithId", compId)
          );
        }
      }

      return Ok(new { Message = "Component updated successfully", Id = id });
    }


    [HttpDelete("{id}")]
    public IActionResult DeleteComponent(int id)
    {
      try
      {
        _db.ExecuteNonQuery(
            "DELETE FROM ComponentCompatibilities WHERE ComponentId = @Id OR CompatibleWithId = @Id",
            new SqlParameter("@Id", id)
        );
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { Message = "Failed to clean up component compatibilities.", Error = ex.Message });
      }

      int rows = _db.ExecuteNonQuery(
          "DELETE FROM Components WHERE Id = @Id",
          new SqlParameter("@Id", id)
      );

      if (rows > 0)
        return Ok(new { Message = "Component deleted successfully" });

      return NotFound(new { Message = "Component not found" });
    }


  }

  public class ComponentDto
  {
    public string Name { get; set; } = "";
    public string? Type { get; set; }
    public decimal UnitPrice { get; set; }
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }

    public List<int>? CompatibleComponentIds { get; set; } = new List<int>();
  }


}