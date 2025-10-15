using Furniture_v1.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data; // Needed for DataRow

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
  private readonly DatabaseHelper _db;

  public AuthController(DatabaseHelper db)
  {
    _db = db;
  }

  [HttpPost("register")]
  public IActionResult Register([FromBody] RegisterDto dto)
  {
    // 1. Check if Email already exists
    var emailCheck = _db.ExecuteQuery(
        "SELECT Id FROM Users WHERE Email = @email",
        new SqlParameter("@email", dto.Email)
    );

    if (emailCheck.Rows.Count > 0)
    {
      return Conflict(new { Message = "Registration failed: User with this email already exists." });
    }

    // 2. Check if Phone number already exists
    var phoneCheck = _db.ExecuteQuery(
        "SELECT Id FROM Users WHERE Phone = @phone",
        new SqlParameter("@phone", dto.Phone)
    );

    if (phoneCheck.Rows.Count > 0)
    {
      return Conflict(new { Message = "Registration failed: User with this phone number already exists." });
    }

    // 3. Proceed with registration since email and phone are unique
    var passwordHash = PasswordHelper.HashPassword(dto.Password);

    var rows = _db.ExecuteNonQuery(
        "INSERT INTO Users (FullName, Email, Phone, Address, Role, Password) VALUES (@name, @email, @phone, @address, @role, @password)",
        new SqlParameter("@name", dto.FullName),
        new SqlParameter("@email", dto.Email),
        new SqlParameter("@phone", dto.Phone),
        new SqlParameter("@address", dto.Address),
        new SqlParameter("@role", dto.Role),
        new SqlParameter("@password", passwordHash)
    );

    if (rows > 0)
      return Ok(new { Message = "User registered successfully" });

    return BadRequest(new { Message = "Registration failed" });
  }

  [HttpPost("login")]
  public IActionResult Login([FromBody] LoginDto dto)
  {
    // NOTE: In a real-world scenario, you should use a SELECT to get the stored hash
    // and then use PasswordHelper.VerifyPassword(dto.Password, storedHash).
    // The current implementation compares a hash of the login password against a stored hash
    // which implies the stored password is also a hash of the login password.
    var passwordHash = PasswordHelper.HashPassword(dto.Password);

    var dt = _db.ExecuteQuery(
        "SELECT * FROM Users WHERE Email = @email AND password = @password",
        new SqlParameter("@email", dto.Email),
        new SqlParameter("@password", passwordHash)
    );

    if (dt.Rows.Count == 0)
      return Unauthorized(new { Message = "Invalid credentials" });

    var user = dt.Rows[0];

    return Ok(new
    {
      Id = user["Id"],
      FullName = user["FullName"],
      Email = user["Email"],
      Phone = user["Phone"],
      Role = user["Role"],
      Address = user["Address"]
    });
  }

  // 💡 UPDATED ENDPOINT
  [HttpPost("forgot-password")]
  public IActionResult ForgotPassword([FromBody] ForgotPasswordDto dto)
  {
    var newPasswordHash = PasswordHelper.HashPassword(dto.Password);

    // Update the password for the user with the given email
    var rowsAffected = _db.ExecuteNonQuery(
        "UPDATE Users SET Password = @newPassword WHERE Email = @email",
        new SqlParameter("@newPassword", newPasswordHash),
        new SqlParameter("@email", dto.Email)
    );

    if (rowsAffected > 0)
      return Ok(new { Message = "Password updated successfully." });

    // If rowsAffected is 0, the user with that email was not found.
    return NotFound(new { Message = "No account found with this email, or password was the same." });
  }

  // 🌟 NEW ENDPOINT
  [HttpPost("check-email")]
  public IActionResult CheckEmail([FromBody] EmailCheckDto dto)
  {
    var dt = _db.ExecuteQuery(
        "SELECT Id FROM Users WHERE Email = @email",
        new SqlParameter("@email", dto.Email)
    );

    if (dt.Rows.Count > 0)
      return Ok(new { Exists = true, Message = "User with this email exists." });

    return Ok(new { Exists = false, Message = "User with this email does not exist." });
  }
}