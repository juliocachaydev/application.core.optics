namespace Jcg.Application.Core.Optics.Tests.TestCommon.Models;

public record Customer
{
    public required Guid CustomerId { get; init; }

    public required string CustomerName { get; init; }

    public required ContactInfo ContactInfo { get; init; }

    public required IEnumerable<Order> Orders { get; init; }

    public static Customer Random => new()
    {
        CustomerId = Guid.NewGuid(),
        CustomerName = "Customer_" + Guid.NewGuid().ToString("N"),
        ContactInfo = new ContactInfo
        {
            Address = new Address
            {
                Street = "Market Street"
            }
        },
        Orders = []
    };
}