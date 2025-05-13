namespace MyAPI.Dtos.Auth
{
    public class TokenDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public int ExpiresIn { get; set; } // Thời gian hết hạn tính bằng giây
    }
}
