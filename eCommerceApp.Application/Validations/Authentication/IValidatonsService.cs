using eCommerceApp.Application.DTOs;
using FluentValidation;

namespace eCommerceApp.Application.Validations.Authentication
{
    public interface IValidationsService
    {
        Task<ServicesResponse> ValidateAsync<T>(T model, IValidator<T> validator);
    }
}
