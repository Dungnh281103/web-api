using System.ComponentModel.DataAnnotations;

public class LoginDto
{
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    public string Password { get; set; }
}
