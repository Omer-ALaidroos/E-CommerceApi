
using eCommerceApp.Domain.Entities.CartEntities;
using eCommerceApp.Domain.Interfaces.CartInterface;
using eCommerceApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Infrastructure.Repository.CartRepo
{
    public class PaymentMethodRepository(AppDbContext context) : IPaymentMethod
    {
        public async Task<IEnumerable<PaymentMethod>> GetPaymentMethods()
        {
            return await context.PaymentMethods.AsNoTracking().ToListAsync();

        }
    }
}
