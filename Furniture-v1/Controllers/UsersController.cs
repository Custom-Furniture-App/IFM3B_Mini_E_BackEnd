using Furniture_v1.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Furniture_v1.Controllers
{
  // DTOs for the controller's use
  public class UserUpdateDto
  {
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
  }

  // NEW DTO for Role Update
  public class UserRoleUpdateDto
  {
    public string Role { get; set; }
  }

  [ApiController]
  [Route("api/[controller]")]
  public class UsersController : ControllerBase
  {
    private readonly DatabaseHelper _db;

    public UsersController(DatabaseHelper db)
    {
      _db = db;
    }

    // ✅ GET: api/users
    [HttpGet]
    public IActionResult GetAll()
    {
      var dt = _db.ExecuteQuery(@"
                SELECT 
                    Id, FullName, Email, Phone, Address, Role, CreatedAt, Disabled, IsDeleted
                FROM Users 
                WHERE IsDeleted = 0
                ORDER BY CreatedAt DESC");

      return Ok(dt.ToDictionaryList());
    }

    // ✅ GET: api/users/{id}
    [HttpGet("{id}")]
    public IActionResult GetSingle(int id)
    {
      var dt = _db.ExecuteQuery(
          @"SELECT Id, FullName, Email, Phone, Address, Role, CreatedAt, Disabled, IsDeleted
                  FROM Users WHERE Id = @id AND IsDeleted = 0",
              new SqlParameter("@id", id)
      );

      if (dt.Rows.Count == 0)
        return NotFound(new { Message = "User not found" });

      return Ok(dt.ToDictionaryList()[0]);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, [FromBody] UserUpdateDto userDto)
    {
      // 1️⃣ Check if user exists
      var dt = _db.ExecuteQuery("SELECT * FROM Users WHERE Id = @id AND IsDeleted = 0", new SqlParameter("@id", id));
      if (dt.Rows.Count == 0)
        return NotFound(new { Message = "User not found or is soft-deleted" });

      // 2️⃣ Dynamically build update query based on provided fields
      var updates = new List<string>();
      var parameters = new List<SqlParameter> { new("@id", id) };

      if (!string.IsNullOrWhiteSpace(userDto.FullName))
      {
        updates.Add("FullName = @fullName");
        parameters.Add(new("@fullName", userDto.FullName));
      }

      if (!string.IsNullOrWhiteSpace(userDto.Email))
      {
        updates.Add("Email = @email");
        parameters.Add(new("@email", userDto.Email));
      }

      if (!string.IsNullOrWhiteSpace(userDto.Phone))
      {
        updates.Add("Phone = @phone");
        parameters.Add(new("@phone", userDto.Phone));
      }

      if (!string.IsNullOrWhiteSpace(userDto.Address))
      {
        updates.Add("Address = @address");
        parameters.Add(new("@address", userDto.Address));
      }

      // 3️⃣ If no fields provided, return a friendly message
      if (updates.Count == 0)
        return BadRequest(new { Message = "No valid fields provided to update." });

      // 4️⃣ Construct the final SQL dynamically
      string sql = $"UPDATE Users SET {string.Join(", ", updates)} WHERE Id = @id AND IsDeleted = 0";

      // 5️⃣ Execute update
      var rows = _db.ExecuteNonQuery(sql, parameters.ToArray());

      if (rows > 0)
      {
        // 6️⃣ Return updated record
        var updatedUser = _db.ExecuteQuery(
            "SELECT Id, FullName, Email, Phone, Address, Role, CreatedAt, Disabled, IsDeleted FROM Users WHERE Id = @id",
            new SqlParameter("@id", id)
        );
        return Ok(new
        {
          Message = "User updated successfully.",
          User = updatedUser.ToDictionaryList()[0]
        });
      }

      return NotFound(new { Message = "User not found or not updated." });
    }


    // 🚀 NEW ENDPOINT: PUT: api/users/updateRole/{id}
    [HttpPut("updateRole/{id}")]
    public IActionResult UpdateRole(int id, [FromBody] UserRoleUpdateDto roleDto)
    {
      // 1. Basic Validation
      if (string.IsNullOrWhiteSpace(roleDto.Role))
      {
        return BadRequest(new { Message = "Role value is required." });
      }

      // Optional: Add validation to ensure the role is one of your allowed roles (e.g., "Manager", "Clerk", "Customer")
      if (roleDto.Role != "Manager" && roleDto.Role != "Clerk")
      {
        return BadRequest(new { Message = $"Invalid role provided: {roleDto.Role}" });
      }

      // 2. Execute the role update
      var rows = _db.ExecuteNonQuery(
          "UPDATE Users SET Role = @role WHERE Id = @id AND IsDeleted = 0",
          new SqlParameter("@role", roleDto.Role),
          new SqlParameter("@id", id)
      );

      // 3. Return result
      if (rows > 0)
      {
        // Retrieve the updated user for the frontend to refresh the view immediately
        var userDt = _db.ExecuteQuery(
            @"SELECT Id, FullName, Email, Phone, Address, Role, CreatedAt, Disabled, IsDeleted
                      FROM Users WHERE Id = @id",
              new SqlParameter("@id", id)
        );

        // Return the full updated user object
        return Ok(userDt.ToDictionaryList()[0]);
      }

      return NotFound(new { Message = "User not found or is soft-deleted" });
    }

    // ✅ PUT: api/users/disable/{id}
    [HttpPut("disable/{id}")]
    public IActionResult DisableUser(int id, [FromBody] bool disable)
    {
      var rows = _db.ExecuteNonQuery(
          "UPDATE Users SET Disabled = @disabled WHERE Id = @id",
          new SqlParameter("@disabled", disable),
          new SqlParameter("@id", id)
      );

      if (rows > 0)
        return Ok(new { Message = disable ? "User disabled successfully" : "User enabled successfully" });

      return NotFound(new { Message = "User not found" });
    }

    // ✅ PUT: api/users/address/{id}
    [HttpPut("address/{id}")]
    public IActionResult UpdateAddress(int id, [FromBody] string newAddress)
    {
      if (string.IsNullOrWhiteSpace(newAddress))
        return BadRequest(new { Message = "Address cannot be empty" });

      var rows = _db.ExecuteNonQuery(
          "UPDATE Users SET Address = @address WHERE Id = @id AND IsDeleted = 0",
          new SqlParameter("@address", newAddress),
          new SqlParameter("@id", id)
      );

      if (rows > 0)
        return Ok(new { Message = "Address updated successfully" });

      return NotFound(new { Message = "User not found" });
    }

    // ✅ DELETE: api/users/{id}
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
      var rows = _db.ExecuteNonQuery(
          "UPDATE Users SET IsDeleted = 1 WHERE Id = @id",
          new SqlParameter("@id", id)
      );

      if (rows > 0)
        return Ok(new { Message = "User deleted successfully" });

      return NotFound(new { Message = "User not found" });
    }
  }
}