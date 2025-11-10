using eCommerceApp.Application.DTOs;
using FluentValidation;

namespace eCommerceApp.Application.Validations.Authentication
{
    public class ValidationsService : IValidationsService
    {
        public async Task<ServicesResponse> ValidateAsync<T>(T model, IValidator<T> validator)
        {
           
            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
               string ErrorToString = string.Join("; ", errors);

                return new ServicesResponse
                {
                    Message = ErrorToString
                };
            }

            return new ServicesResponse {IsSuccess = true };
        }
    }
}
