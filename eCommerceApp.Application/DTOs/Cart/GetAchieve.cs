namespace eCommerceApp.Application.DTOs.Cart
{
    public class GetAchieve
    {
        public string ? ProductName { get; set; }
        public int QuantityOrderd { get; set; }

        public decimal AmountPayed { get; set; }

        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public DateTime? DatePurchased { get; set; }
    }
}
