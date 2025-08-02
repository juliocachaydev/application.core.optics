namespace Jcg.Application.Optics.Tests.TestingCommon;

public record Order
{
    public required Guid OrderId { get; init; }

    public required int OrderNumber { get; init; }

    public required IEnumerable<OrderLine> Lines { get; init; }

    public static Order Random => new()
    {
        OrderId = Guid.NewGuid(),
        OrderNumber = RandomHelper.GenInt(1000, 100_000),
        Lines = []
    };
}