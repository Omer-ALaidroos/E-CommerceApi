using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.Services.Interfaces.Authentication;
using eCommerceApp.Application.Services.Interfaces.Logger;
using eCommerceApp.Application.Services.Interfaces;
using eCommerceApp.Application.Validations.Authentication;
using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Interfaces.Authentication;
using Microsoft.AspNetCore.Identity;
using FluentValidation;
using eCommerceApp.Application.DTOs.Identity; // For getting frontend URL

namespace eCommerceApp.Application.Services.Implementation
{
    public class AuthenticationService
    (IUserManagement userManagement,
     ITokenManagements tokenManagements,
      IRoleManagement roleManagement,
      IAppLogger<AuthenticationService> logger,
      IMapper mapper,
      IValidator<CreateUser> createUserValidator,
      IValidator<LoginUser> loginUserValidator,
      IValidator<ChangePassword> changePasswordValidator,
      IValidator<ForgotPasswordDto> forgotPasswordValidator, // New validator
      IValidator<ResetPasswordDto> resetPasswordValidator,   // New validator
      IValidationsService validationsService,
      IEmailService emailService, // New Email Service
      UserManager<AppUser> userManager,
      IPasswordResetOtpRepository otpRepository
     )
         : IAuthenticationService
    {
        public async Task<ServicesResponse> ChangePassword(ChangePassword changePassword, string userId)
        {
            var validationResult = await validationsService.ValidateAsync(changePassword, changePasswordValidator);
            if (!validationResult.IsSuccess) return validationResult;

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return new ServicesResponse(Message: "User not found.");

            var result = await userManager.ChangePasswordAsync(user, changePassword.CurrentPassword, changePassword.NewPassword);
            if (!result.Succeeded)
            {
                var error = string.Join("; ", result.Errors.Select(e => e.Description));
                return new ServicesResponse(Message: error);
            }

            return new ServicesResponse(IsSuccess: true, Message: "Password changed successfully.");
        }

        public async Task<ServicesResponse> CreateUser(CreateUser user)
        {
            var validationResult = await validationsService.ValidateAsync(user, createUserValidator);
            if (!validationResult.IsSuccess) return validationResult;

            var mapperUser = mapper.Map<AppUser>(user);
           mapperUser.UserName = user.Email;
           mapperUser.PasswordHash= user.Password;

            var result = await userManagement.CreateUser(mapperUser);
            if (!result)
                return new ServicesResponse(Message: "Email address is already in use or an unknown error occurred.");

            

            var assignRoleResult = await roleManagement.AddUserToRole(mapperUser,  "User");
            if (!assignRoleResult)
            {
                int removeResult = await userManagement.RemoveUserByEmail(mapperUser.Email!);
                if (removeResult <= 0)
                {
                    logger.LogError(
                        new Exception($"Failed to assign role to user {mapperUser.Email} and also failed to remove the user."),
                        "User could not be assigned role");
                    return new ServicesResponse(Message: "An unknown error occurred in creating account. Please contact support.");
                }

                
            }
                
           return new ServicesResponse(IsSuccess: true, Message: "Account Created");
        }

        public async Task<LoginResponse> LoginUser(LoginUser user)
        {
             var validationResult = await validationsService.ValidateAsync(user, loginUserValidator);
            if (!validationResult.IsSuccess)
                return new LoginResponse(Message: validationResult.Message);

            var mappedModel = mapper.Map<AppUser>(user);
            mappedModel.PasswordHash = user.Password;
            var loginResult = await userManagement.LoginUser(mappedModel);
            if (!loginResult)
                return new LoginResponse(Message: "Email not found or invalid Password.");

            var _user = await userManagement.GetUserByEmail(user.Email!);
            var cliams = await userManagement.GetUserClaims(_user!.Email!);

            string jwtToken = tokenManagements.GenerateToken(cliams);
            string refreshToken = tokenManagements.GetRefreshToken();

            int SaveTokenResult = 0;
            bool UserTokenCheck = await tokenManagements.ValidateRefreshToken(refreshToken);
            if (UserTokenCheck)
                SaveTokenResult = await tokenManagements.UpdateRefreshToken(_user.Id!, refreshToken);
            else
                SaveTokenResult = await tokenManagements.AddRefreshToken(_user.Id!, refreshToken);

            if (SaveTokenResult <= 0)
                return new LoginResponse(Message: "internal Error occured While Authentication.");

            return new LoginResponse(Success: true, Token: jwtToken, Refreshtoken: refreshToken, UserId: _user.Id);

        }

        public async Task<LoginResponse> ReviveToken(string RefreshToken)
        {
            bool VslidateTokenResult = await tokenManagements.ValidateRefreshToken(RefreshToken);
            if (!VslidateTokenResult)
                return new LoginResponse(Message: "Invalid Refresh Token");

            string userId = await tokenManagements.GetUserIdByRefreshToken(RefreshToken);
            var _user = await userManagement.GetUserById(userId);
            var claims = await userManagement.GetUserClaims(_user.Email!);

            string jwtToken = tokenManagements.GenerateToken(claims);
            string newRefreshToken = tokenManagements.GetRefreshToken();
            await tokenManagements.UpdateRefreshToken(userId, newRefreshToken);
            return new LoginResponse(Success: true, Token: jwtToken, Refreshtoken: newRefreshToken);
        }

        public async Task<ServicesResponse> ForgotPassword(ForgotPasswordDto model)
        {
            // Validate the email
            var validationResult = await validationsService.ValidateAsync(model, forgotPasswordValidator);
            if (!validationResult.IsSuccess) return validationResult;

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                logger.LogInformation($"Forgot password requested for non-existent email: {model.Email}");
                return new ServicesResponse(true, "If an account exists with that email, a verification code has been sent.");
            }

           
            await otpRepository.InvalidateOldOtpsAsync(user.Id);

          
            var code = Random.Shared.Next(100000, 999999).ToString();
            var newOtp = new PasswordResetOtp
            {
                UserId = user.Id,
                Code = code,
                ExpireAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                AttemptsCount = 0
            };

            await otpRepository.AddAsync(newOtp);

            var subject = "Your Password Reset Code";
            var body = $"Your verification code is: <strong>{code}</strong>. It expires in 10 minutes.";

            await emailService.SendEmailAsync(user.Email!, subject, body);

            return new ServicesResponse(true, "If an account exists with that email, a verification code has been sent.");
        }

        public async Task<ServicesResponse> VerifyResetCode(VerifyResetCodeDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null) return new ServicesResponse(false, "Invalid code or email.");

            var otp = await otpRepository.GetLatestActiveOtpAsync(user.Id);

            if (otp == null || otp.ExpireAt < DateTime.UtcNow)
                return new ServicesResponse(false, "Code has expired or is invalid.");

            if (otp.AttemptsCount >= 5)
            {
                otp.IsUsed = true;
                await otpRepository.UpdateAsync(otp);
                return new ServicesResponse(false, "Too many failed attempts. Please request a new code.");
            }

            if (otp.Code != model.Code)
            {
                otp.AttemptsCount++;
                await otpRepository.UpdateAsync(otp);
                return new ServicesResponse(false, "Invalid verification code.");
            }

            return new ServicesResponse(true, "Code verified successfully.");
        }

        public async Task<ServicesResponse> ResetPassword(ResetPasswordDto model)
        {
            var validationResult = await validationsService.ValidateAsync(model, resetPasswordValidator);
            if (!validationResult.IsSuccess) return validationResult;

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null) return new ServicesResponse(false, "Invalid request.");

            var otp = await otpRepository.GetActiveOtpAsync(user.Id, model.Code);

            if (otp == null || otp.ExpireAt < DateTime.UtcNow)
                return new ServicesResponse(false, "Invalid or expired code.");

          
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (!result.Succeeded)
            {
                var error = string.Join("; ", result.Errors.Select(e => e.Description));
                return new ServicesResponse(false, error);
            }

            otp.IsUsed = true;
            await otpRepository.UpdateAsync(otp);

            return new ServicesResponse(true, "Password has been reset successfully.");
        }
    }
}