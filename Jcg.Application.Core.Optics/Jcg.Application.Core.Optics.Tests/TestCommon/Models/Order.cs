namespace Jcg.Application.Core.Optics.Tests.TestCommon.Models;

public record Order
{
    public required Guid OrderId { get; init; }

    public required int Number { get; init; }

    public required IEnumerable<OrderItem> Items { get; init; }

    public static Order Random => new()
    {
        OrderId = Guid.NewGuid(),
        Number = CustomRandom.RandomInt(),
        Items = []
    };
}