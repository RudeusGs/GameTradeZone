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
using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore;

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
                var userExistByUsername = await _userManager.FindByNameAsync(model.UserName);
                if (userExistByUsername != null)
                {
                    return new ApiResult()
                    {
                        Message = $"{model.UserName} đã được sử dụng. Vui lòng thử lại!"
                    };
                }

                var userExistByEmail = await _userManager.FindByEmailAsync(model.Email);
                if (userExistByEmail != null)
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
                    IsAuthen = false
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

                return new ApiResult();
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
        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
        private void SendOtpEmail(string email, string otp)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("gametradezone.gtz@gmail.com"),
                Subject = "Your OTP Code",
                Body = $"Your OTP code is {otp}. It will expire in 5 minutes.",
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("gametradezone.gtz@gmail.com", "vgcz gkhd xoic rgmd"),
                EnableSsl = true
            };

            smtpClient.Send(mailMessage);
        }
        public async Task<ApiResult> SendOtpForEmailVerificationAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new ApiResult { Message = "User not found" };
                }

                if (user.IsAuthen)
                {
                    return new ApiResult { Message = "Email is already verified." };
                }

                var otpCode = GenerateOtp();
                var existingOtps = await _dbContext.OTPs
                    .Where(o => o.UserId == user.Id)
                    .ToListAsync();
                _dbContext.OTPs.RemoveRange(existingOtps);

                var otpEntity = new OTPs
                {
                    UserId = user.Id,
                    OTP = otpCode,
                    Email = email,
                    ExpirationTime = DateTime.Now.AddMinutes(5)
                };

                await _dbContext.OTPs.AddAsync(otpEntity);
                await _dbContext.SaveChangesAsync();
                SendOtpEmail(email, otpCode);

                return new ApiResult()
                {
                    Data = new { Message = "OTP sent successfully." }
                };
            }
            catch (Exception ex)
            {
                return new ApiResult { Message = $"An error occurred while sending OTP: {ex.Message}" };
            }
        }
        public async Task<ApiResult> VerifyOtpForEmailVerificationAsync(VerifyOtpModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null || user.DeleteDate.HasValue)
                {
                    return new ApiResult { Message = "User not found or has been deleted." };
                }

                var otpRecord = await _dbContext.OTPs
                    .Where(o => o.UserId == user.Id && o.OTP == model.OTP && o.ExpirationTime > DateTime.Now)
                    .FirstOrDefaultAsync();

                if (otpRecord == null)
                {
                    return new ApiResult { Message = "Invalid or expired OTP." };
                }

                user.IsAuthen = true;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    return new ApiResult { Message = "Error updating user verification status." };
                }
                _dbContext.OTPs.Remove(otpRecord);
                await _dbContext.SaveChangesAsync();

                return new ApiResult()
                {
                    Data = new { Message = "Email verified successfully." }
                };
            }
            catch (Exception ex)
            {
                return new ApiResult { Message = $"An error occurred while verifying OTP: {ex.Message}" };
            }
        }
        public async Task<ApiResult> SendCustomEmailAsync(int userId, string subject, string messageBody)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null || user.DeleteDate.HasValue)
                {
                    return new ApiResult { Message = "Người dùng không tồn tại hoặc đã bị xóa." };
                }

                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    return new ApiResult { Message = "Người dùng không có địa chỉ email hợp lệ." };
                }

                if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(messageBody))
                {
                    return new ApiResult { Message = "Tiêu đề và nội dung email không được để trống." };
                }

                // Kiểm tra độ dài chuỗi để tránh lỗi cơ sở dữ liệu
                if (subject.Length > 255)
                {
                    return new ApiResult { Message = "Tiêu đề email không được dài quá 255 ký tự." };
                }

                if (user.Email.Length > 255)
                {
                    return new ApiResult { Message = "Địa chỉ email người nhận không được dài quá 255 ký tự." };
                }
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("gametradezone.gtz@gmail.com"),
                    Subject = subject,
                    Body = messageBody,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(user.Email);

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("gametradezone.gtz@gmail.com", "vgcz gkhd xoic rgmd"),
                    EnableSsl = true
                };

                // Tạo bản ghi EmailLog
                var emailLog = new EmailLog
                {
                    UserId = userId,
                    Subject = subject,
                    MessageBody = messageBody,
                    SenderEmail = "gametradezone.gtz@gmail.com",
                    ReceiverEmail = user.Email,
                    SentDate = DateTime.Now,
                    IsSuccess = true, // Sẽ cập nhật nếu gửi thất bại
                    CreatedDate = DateTime.Now
                };

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    emailLog.IsSuccess = false;
                    emailLog.ErrorMessage = ex.Message.Length > 1000 ? ex.Message.Substring(0, 1000) : ex.Message;
                }

                // Lưu bản ghi vào cơ sở dữ liệu
                try
                {
                    await _dbContext.EmailLogs.AddAsync(emailLog);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception dbEx)
                {
                    var innerException = dbEx.InnerException?.Message ?? dbEx.Message;
                    return new ApiResult { Message = $"Lỗi khi lưu bản ghi email vào cơ sở dữ liệu: {innerException}" };
                }

                if (!emailLog.IsSuccess)
                {
                    return new ApiResult { Message = $"Lỗi khi gửi email: {emailLog.ErrorMessage}" };
                }

                return new ApiResult
                {
                    Data = new { Message = "Gửi email thành công." }
                };
            }
            catch (Exception ex)
            {
                return new ApiResult { Message = $"Lỗi khi gửi email: {ex.Message}" };
            }
        }
    }
}