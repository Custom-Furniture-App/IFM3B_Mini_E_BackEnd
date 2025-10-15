public class RegisterDto
{
  public string FullName { get; set; } = "";
  public string Email { get; set; } = "";
  public string Phone { get; set; } = "";
  public string Address { get; set; } = "";
  public string Password { get; set; } = "";
  public string Role { get; set; } = "";

}

public class LoginDto
{
  public string Email { get; set; } = "";
  public string Password { get; set; } = "";
}

public class ForgotPasswordDto
{
  public string Email { get; set; }
  public string Password { get; set; } // The new password
}

public class EmailCheckDto
{
  public string Email { get; set; }
}
