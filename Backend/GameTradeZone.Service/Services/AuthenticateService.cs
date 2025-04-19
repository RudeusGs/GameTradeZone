using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Constants;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models.Authenticate;
using GameTradeZone.Service.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace GameTradeZone.Service.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly DataContext _dbContext;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticateService(
            DataContext dbContext,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole<int>> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public async Task<ApiResult> ExternalLoginAsync(ExternalLoginInfo info)
        {
            try
            {
                if (info == null)
                {
                    return new ApiResult { Message = "Không thể lấy thông tin đăng nhập từ Google." };
                }

                // Thử đăng nhập nếu tài khoản đã liên kết
                var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                    var roles = await _userManager.GetRolesAsync(user);
                    var roleString = string.Join(",", roles);
                    var token = GenerateUserToken(user, roleString, true);
                    return new ApiResult { Data = token };
                }

                // Lấy email từ Google
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(email))
                {
                    return new ApiResult { Message = "Không thể lấy email từ Google." };
                }

                // Kiểm tra xem email đã tồn tại chưa
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    return new ApiResult { Message = "Email đã được sử dụng bởi tài khoản khác. Vui lòng đăng nhập bằng phương thức khác." };
                }

                // Tạo tài khoản mới
                var newUser = new User
                {
                    UserName = email,
                    Email = email,
                    FullName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Balance = 10000,
                    Coin = 2,
                    Level = 0,
                    Experience = 0,
                    Status = true,
                    CreatedDate = DateTime.Now,
                    BankName = "",
                    BankNumber = ""
                };

                using var transaction = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    var createResult = await _userManager.CreateAsync(newUser);
                    if (!createResult.Succeeded)
                    {
                        return new ApiResult { Message = string.Join('\n', createResult.Errors.Select(x => x.Description)) };
                    }

                    await _userManager.AddLoginAsync(newUser, info);
                    if (!await _roleManager.RoleExistsAsync(RoleConstants.USER.ToString()))
                    {
                        await _roleManager.CreateAsync(new IdentityRole<int>(RoleConstants.USER.ToString()));
                    }
                    await _userManager.AddToRoleAsync(newUser, RoleConstants.USER.ToString());

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    var roles = await _userManager.GetRolesAsync(newUser);
                    var roleString = string.Join(",", roles);
                    var token = GenerateUserToken(newUser, roleString, true);
                    return new ApiResult { Data = token };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ApiResult { Message = $"Lỗi khi tạo tài khoản: {ex.Message}" };
                }
            }
            catch (Exception ex)
            {
                return new ApiResult { Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task<ApiResult> Login(LoginModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user != null && !user.DeleteDate.HasValue)
                {
                    var signInResult = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, lockoutOnFailure: false);
                    if (signInResult.Succeeded)
                    {
                        var role = await _userManager.GetRolesAsync(user);
                        var roleString = String.Join(",", role.ToArray());
                        var tokenResult = GenerateUserToken(user, roleString);
                        if (tokenResult != null)
                        {
                            return new ApiResult()
                            {
                                Data = tokenResult,
                            };
                        }
                    }
                }

                return new ApiResult()
                {
                    Message = "Unorthorize",
                };
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public async Task<ApiResult> LoginAsAdmin(LoginModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user != null && !user.DeleteDate.HasValue)
                {
                    var signInResult = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, lockoutOnFailure: false);
                    if (signInResult.Succeeded)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Contains("Admin"))
                        {
                            var tokenResult = GenerateUserToken(user, "Admin");
                            if (tokenResult != null)
                            {
                                return new ApiResult()
                                {
                                    Data = tokenResult
                                };
                            }
                        }
                    }
                }
                return new ApiResult()
                {
                    Message = "Unauthorized",
                };
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public async Task<ApiResult> Register(RegisterModel model)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var userExist = await _userManager.FindByNameAsync(model.UserName);
                if (userExist != null)
                {
                    return new ApiResult()
                    {
                        Message = $"{model.Email} đã được sử dụng. Vui lòng thử lại!"
                    };
                }

                User user = new User()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Balance = 10000,
                    Coin = 2,
                    BankName = model.BankName,
                    BankNumber = model.BankNumber,
                    Level = 0,
                    Experience = 0,
                    Status = false,
                    CreatedDate = DateTime.Now,
                };

                var newUserResult = await _userManager.CreateAsync(user, model.Password);

                if (!newUserResult.Succeeded)
                {
                    var err = newUserResult.Errors.Select(x => x.Description);
                    return new() { Message = string.Join('\n', err) };
                }

                if (!await _roleManager.RoleExistsAsync(RoleConstants.USER.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole<int>(RoleConstants.USER.ToString()));
                }

                await _userManager.AddToRoleAsync(user, RoleConstants.USER.ToString());

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ApiResult()
                {
                };
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new Exception(e.ToString());
            }
        }

        private UserToken GenerateUserToken(User user, string role, bool isExternalLogin = false)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var expires = DateTime.UtcNow.AddHours(7).AddDays(7);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.AddHours(7).ToString()),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.NameIdentifier, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                }),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Issuer = _configuration["Jwt:ValidIssuer"],
                Audience = _configuration["Jwt:ValidAudience"]
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return new UserToken
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Token = token,
                Expires = expires
            };
        }

        public async Task<ApiResult> GetUserRoles(int userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null || user.DeleteDate.HasValue)
                {
                    return new ApiResult()
                    {
                        Message = "User not found or has been deleted."
                    };
                }

                var roles = await _userManager.GetRolesAsync(user);
                return new ApiResult()
                {
                    Data = roles
                };
            }
            catch (Exception e)
            {
                return new ApiResult()
                {
                    Message = "An error occurred while fetching user roles."
                };
            }
        }
        public async Task<ApiResult> UpdateUser(string userId, UpdateUserModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.DeleteDate.HasValue)
                {
                    return new ApiResult { Message = "User not found or has been deleted." };
                }
                var userExist = await _userManager.FindByNameAsync(model.UserName);
                if (userExist != null)
                {
                    return new ApiResult()
                    {
                        Message = $"{model.Email} đã được sử dụng. Vui lòng thử lại!"
                    };
                }
                user.UserName = model.UserName;
                user.FullName = model.FullName;
                user.Email = model.Email;
                user.BankName = model.BankName;
                user.BankNumber = model.BankNumber;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    return new ApiResult { Message = string.Join('\n', updateResult.Errors.Select(x => x.Description)) };
                }

                await _dbContext.SaveChangesAsync();
                var roles = await _userManager.GetRolesAsync(user);
                var roleString = string.Join(",", roles);
                var token = GenerateUserToken(user, roleString, true);

                return new ApiResult
                {
                    Data = new
                    {
                        userId = user.Id,
                        username = user.UserName,
                        email = user.Email,
                        fullName = user.FullName,
                        bankname = user.BankName,
                        banknumber = user.BankNumber,
                        balance = user.Balance ?? 0,
                        coin = user.Coin ?? 0
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResult { Message = $"Lỗi khi cập nhật thông tin: {ex.Message}" };
            }
        }
    }
}