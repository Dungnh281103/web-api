namespace MyAPI.Dtos.Auth
{
    public class JwtTokenSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpireInHours { get; set; }
    }

}
