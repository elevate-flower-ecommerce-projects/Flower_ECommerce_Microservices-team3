namespace Payment_Service.Application.Abstractions
{
    public interface IPaymobCheckoutUrlBuilder
    {
        string Build(string clientSecret);
    }
}
