using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyAPI.Data;
using MyAPI.Dtos;
using MyAPI.Dtos.Auth;
using MyAPI.Dtos.Common;
using MyAPI.Extension;
using MyAPI.Services.Token;
using System;
using System.Security.Claims;
using System.Threading.Tasks;


namespace TravelApp.Api.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthController(
            TokenService tokenService,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize]
        [HttpGet("/user/profile")]
        public async Task<IActionResult> Profile()
        {
            var currentUserId = User.GetUserId();
            var result = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResult<string>(400, "Dữ liệu không hợp lệ"));

            // Kiểm tra email đã tồn tại chưa
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return BadRequest(new ApiErrorResult<string>(400, "Email đã được đăng ký"));

            // Tạo username theo định dạng 
            string username = model.Nickname ;

            // Tạo user mới
            var user = new AppUser
            {
                UserName = username, // Using the new username format
                Email = model.Email,
                Nickname = model.Nickname,
                EmailConfirmed = true // For testing, set to true
            };

            // Lưu user vào database
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new ApiErrorResult<string>(400, $"Đăng ký thất bại: {errors}"));
            }

            // Trả về kết quả thành công
            return Ok(new ApiSuccessResult<string>(200, "Đăng ký thành công! Bây giờ bạn đã có thể đăng nhập."));
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResult<TokenDto>(400, "Dữ liệu không hợp lệ"));

            // Tìm user theo email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new ApiErrorResult<TokenDto>(401, "Email hoặc mật khẩu không đúng"));

            // Kiểm tra mật khẩu
            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // Ghi nhận thất bại đăng nhập
                await _userManager.AccessFailedAsync(user);
                return Unauthorized(new ApiErrorResult<TokenDto>(401, "Email hoặc mật khẩu không đúng"));
            }

            // Kiểm tra tài khoản bị khóa
            if (await _userManager.IsLockedOutAsync(user))
                return Unauthorized(new ApiErrorResult<TokenDto>(401, "Tài khoản đã bị khóa. Vui lòng thử lại sau."));

            // Reset số lần thất bại
            await _userManager.ResetAccessFailedCountAsync(user);

            // Tạo token
            var token = _tokenService.GenerateToken(user);

            // Lưu refresh token vào DB
            user.RefreshToken = token.RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            // Trả về token
            return Ok(new ApiSuccessResult<TokenDto>(200, token));
        }


        // Trong AuthController (hoặc UserController)
        [Authorize]
        [HttpPut("/user/profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto dto)
        {
            var currentUserId = User.GetUserId();
            var user = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (user == null)
                return Unauthorized(new ApiErrorResult<string>(401, "Người dùng không tồn tại"));

            // 1) Xử lý upload file
            if (dto.AvatarFile != null && dto.AvatarFile.Length > 0)
            {
                // Thư mục lưu file (wwwroot/uploads)
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Tạo tên file duy nhất
                var ext = Path.GetExtension(dto.AvatarFile.FileName);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Lưu file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.AvatarFile.CopyToAsync(stream);
                }

                // Gán URL cho user
                user.Avatar = $"/uploads/{fileName}";
            }

            // 2) Ánh xạ các trường còn lại
            user.Nickname = dto.Nickname;
            user.Dob = dto.Dob;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join("; ", updateResult.Errors.Select(e => e.Description));
                return BadRequest(new ApiErrorResult<string>(400, $"Cập nhật thất bại: {errors}"));
            }

            // 3) Trả về profile mới
            var updatedProfile = new
            {
                id = user.Id,
                nickname = user.Nickname,
                avatar = user.Avatar,
                dob = user.Dob,
                email = user.Email,
                username = user.UserName,
                createdAt = user.CreatedAt
            };
            return Ok(new ApiSuccessResult<object>(200, updatedProfile));
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenDto tokenDto)
        {
            if (tokenDto == null || string.IsNullOrEmpty(tokenDto.AccessToken) || string.IsNullOrEmpty(tokenDto.RefreshToken))
                return BadRequest(new ApiErrorResult<TokenDto>(400, "Token không hợp lệ"));

            try
            {
                // Validate access token cũ
                var principal = _tokenService.GetPrincipalFromExpiredToken(tokenDto.AccessToken);
                var email = principal.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(email))
                    return BadRequest(new ApiErrorResult<TokenDto>(400, "Email không tồn tại trong token"));

                // Tìm user theo email
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return BadRequest(new ApiErrorResult<TokenDto>(400, "Token không hợp lệ"));

                // Kiểm tra refresh token
                if (user.RefreshToken != tokenDto.RefreshToken ||
                    user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    return BadRequest(new ApiErrorResult<TokenDto>(400, "Refresh token không hợp lệ hoặc đã hết hạn"));
                }

                // Tạo token mới
                var newToken = _tokenService.GenerateToken(user);

                // Cập nhật refresh token
                user.RefreshToken = newToken.RefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _userManager.UpdateAsync(user);

                // Trả về token mới
                return Ok(new ApiSuccessResult<TokenDto>(200, newToken));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResult<TokenDto>(400, $"Làm mới token thất bại: {ex.Message}"));
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Lấy username từ token hiện tại
                var username = User.Identity.Name;

                // Tìm user theo username
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                    return BadRequest(new ApiErrorResult<string>(400, "Người dùng không tồn tại"));

                // Xóa refresh token
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _userManager.UpdateAsync(user);

                return Ok(new ApiSuccessResult<string>(200, "Đăng xuất thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResult<string>(400, $"Đăng xuất thất bại: {ex.Message}"));
            }
        }
    }
}
