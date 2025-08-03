namespace Jcg.Application.Core.Optics.Tests.TestCommon.Models;

public record OrderItem
{
    public required string ProductName { get; init; }
    public required int Quantity { get; init; }
    public required decimal Price { get; init; }

    public static OrderItem Random => new()
    {
        ProductName = CustomRandom.RandomString(),
        Quantity = CustomRandom.RandomInt(),
        Price = CustomRandom.RandomDecimal()
    };
}