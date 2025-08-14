using FlockStudy.Data;
using FlockStudy.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace FlockStudy.Service
{
    public interface IUserService
    {
        Task<bool> LoginAsync(string email, string password);
        Task LogoutAsync();
        Task<User> GetCurrentUserAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> CreateUserAsync(User user, string password);
        Task<int> GetCurrentUserId();
    }
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordService _passwordService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserService> _logger;

        public UserService(
            ApplicationDbContext context,
            IPasswordService passwordService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UserService> logger)
        {
            _context = context;
            _passwordService = passwordService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var user = await GetUserByEmailAsync(email);
                if (user == null || !_passwordService.VerifyPassword(password, user.PasswordHash))
                {
                    _logger.LogWarning("Failed login attempt for email: {Email}", email);
                    return false;
                }

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
            };

                // Create identity and principal
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                };

                // Sign in the user
                await _httpContextAccessor.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                _logger.LogInformation("User {Email} logged in successfully", email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", email);
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                var currentUser = await GetCurrentUserAsync();
                await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                if (currentUser != null)
                {
                    _logger.LogInformation("User {Email} logged out", currentUser.Email);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
            }
        }

        public async Task<User> GetCurrentUserAsync()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext?.User?.Identity?.IsAuthenticated != true)
                {
                    return null;
                }

                var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return null;
                }

                return await GetUserByIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return null;
            }
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            try
            {
                return await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {UserId}", id);
                return null;
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return null;

                return await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
                return null;
            }
        }

        public async Task<bool> CreateUserAsync(User user, string password)
        {
            try
            {

                var existingUser = await GetUserByEmailAsync(user.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("Attempted to create user with existing email: {Email}", user.Email);
                    return false;
                }

                var existingUsername = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username.ToLower() == user.Username.ToLower());

                if (existingUser != null)
                {
                    _logger.LogWarning("Attempted to create user with existing username: {username}", user.Username);
                    return false;
                }

                // Hash password
                user.PasswordHash = _passwordService.HashPassword(password);

                user.CreatedAt = DateTime.UtcNow;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User created successfully: {Email}", user.Email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {Email}", user.Email);
                return false;
            }
        }

        public async Task<int> GetCurrentUserId()
        {
 
            var httpContext = _httpContextAccessor.HttpContext;
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return 0;
            }
            int userId = int.Parse(userIdClaim);
            return userId;
        }
    }
}
