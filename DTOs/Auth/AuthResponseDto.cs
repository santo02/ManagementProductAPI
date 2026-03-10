namespace ManagementProduct.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string AuthenticationToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
