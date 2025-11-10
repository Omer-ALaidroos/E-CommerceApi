namespace eCommerceApp.Application.Services.Interfaces.Logger
{
    public interface IAppLogger<T>
    {
        public void LogInformation(string message);
        public void LogWarning(string message);
        public void LogError(Exception ex,string message);
    }
}
