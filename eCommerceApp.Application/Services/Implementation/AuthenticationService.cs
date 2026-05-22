using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.Services.Interfaces.Authentication;
using eCommerceApp.Application.Services.Interfaces.Logger;
using eCommerceApp.Application.Validations.Authentication;
using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Interfaces.Authentication;
using FluentValidation;

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
      IValidationsService validationsService)
         : IAuthenticationService
    {
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

            //look at this
            var users = await userManagement.GetAllUsers();

            var assignRoleResult = await roleManagement.AddUserToRole(mapperUser, users!.Count() == 1 ? "Admin" : "User");
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
                return new LoginResponse(Message: "Email not found or invalid credentials.");

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
    }
}