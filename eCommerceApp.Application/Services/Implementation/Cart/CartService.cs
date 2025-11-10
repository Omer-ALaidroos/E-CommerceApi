using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Application.Services.Interfaces.Cart;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Entities.Cart;
using eCommerceApp.Domain.Interfaces;
using eCommerceApp.Domain.Interfaces.Authentication;
using eCommerceApp.Domain.Interfaces.Cart;

namespace eCommerceApp.Application.Services.Implementation.Cart
{
    public class CartService(ICart cartInterface, IMapper mapper,
     IGeneric<Product> ProductInterface,IPaymentMethodService paymentMethodService
     ,IPaymentService paymentService ,IUserManagement userManagement ) : ICartService
    {
        public async Task<ServicesResponse> Checkout(Checkout checkout)
        {
           var (products, totalAmount) = await GetCartTotalAmount(checkout.Carts);

            var PaymentMethods = await paymentMethodService.GetPaymntMethods();
            if (checkout.PaymentMethodId == PaymentMethods.FirstOrDefault()!.Id)
            {
                return await paymentService.Pay(totalAmount, products, checkout.Carts);

            }
            else
            {
                return new ServicesResponse
                {
                    IsSuccess = false,
                    Message = "Invalid Payment Method"
                };
            }
            
           
        }

        public async Task<ServicesResponse> SaveCheckoutHistory(IEnumerable<CreateAchieve> achieves)
        {
            var mappedData = mapper.Map<IEnumerable<Achieve>>(achieves);

            var result = await cartInterface.SaveCheckoutHistory(mappedData);
            return result > 0 ? new ServicesResponse
            {
                IsSuccess = true,
                Message = "Checkout Achieved",

            } : new ServicesResponse
            {
                IsSuccess = false,
                Message = "Errror occured while saving achieves",

            };

        }
        public async Task<IEnumerable<GetAchieve>> GetAllCheckoutHistory()
        {
            var history = await cartInterface.GetAllCheckoutHistory();
            if (history == null) return [];

            var GroupByCustomerID = history.GroupBy(h => h.UserId).ToList();
            var Products = await ProductInterface.GetAllAsync();
            var Achieves = new List<GetAchieve>();

            foreach (var customerId in GroupByCustomerID)
            {
                var CustomerDetails = await userManagement.GetUserById(customerId.Key!);
                foreach (var item in customerId)
                {
                    var product = Products.FirstOrDefault(p => p.Id == item.productId);
                    Achieves.Add(new GetAchieve
                    {
                        ProductName = product?.Name,
                        QuantityOrderd = item.quantity,
                        AmountPayed = item.quantity * product!.Price,
                        CustomerName = CustomerDetails?.FullName,
                        CustomerEmail = CustomerDetails?.Email,
                        DatePurchased = item.createdDate
                    });
                }

            }
            
            return Achieves;
        }
        
        private async Task<(IEnumerable<Product>,decimal)> GetCartTotalAmount(IEnumerable<ProcessCart> carts)
        {
            if (!carts.Any())
                return ([], 0);

            var Products = await ProductInterface.GetAllAsync();

            if (!Products.Any())
                return ([], 0);

            var CartProducts = carts
            .Select(cartItem => Products.FirstOrDefault(p => p.Id == cartItem.ProductId))
            .Where(p => p != null)
            .ToList();

            var TotalAmount = carts
            .Where(cartITem => CartProducts.Any(p => p.Id == cartITem.ProductId))
            .Sum(cartItem => cartItem.Quantity *
            (CartProducts.First(p => p.Id == cartItem.ProductId)!.Price));
           
            return (CartProducts!, TotalAmount);
        }

        public async Task<IEnumerable<GetAchieve>> GetAchieves()
        {
            var achieves =await cartInterface.GetAllCheckoutHistory();

            if (!achieves.Any()) return [];

           var mapperData = mapper.Map<IEnumerable<GetAchieve>>(achieves);
            return mapperData;
        }
    }
}
