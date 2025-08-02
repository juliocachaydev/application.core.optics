namespace Jcg.Application.Optics.Tests.TestingCommon;

public record Customer
{
    public required Guid CustomerId { get; init; }

    public required string Name { get; init; }

    public required ContactInfo ContactInfo { get; init; }

    public required IEnumerable<Order> Orders { get; init; }
    
    public static Customer Random => new()
    {
        CustomerId = Guid.NewGuid(),
        Name = RandomHelper.GenString(),
        ContactInfo = new()
        {
            ContactInfoId = Guid.NewGuid(),
            Address = new()
            {
                Street = RandomHelper.GenString(5, 15),
                HouseNumber = RandomHelper.GenInt(100, 999)
            },
            Phone = "(423) 555-5555"
        },
        Orders = []
    };
}

public record ContactInfo
{
    public required Guid ContactInfoId { get; init; }

    public required Address Address { get; init; }

    public required string Phone { get; init; }
}

public record Address
{
    public required string Street { get; init; }

    public required int HouseNumber { get; init; }
}