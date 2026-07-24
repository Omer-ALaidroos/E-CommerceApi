namespace eCommerceApp.Application.Contracts.Payment
{
    public class PaymentResult<T>
    {
        public bool IsSuccess { get; init; }
        public string Message { get; init; } = string.Empty;
        public T? Data { get; init; }
        public string? ErrorCode { get; init; }

        public static PaymentResult<T> Success(T data, string message = "Operation completed successfully.")
            => new()
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };

        public static PaymentResult<T> Failure(string message, string? errorCode = null)
            => new()
            {
                IsSuccess = false,
                Message = message,
                ErrorCode = errorCode
            };
    }
}
