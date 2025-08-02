namespace Jcg.Application.Optics.Tests.TestingCommon;

public record OrderLine
{
    public required Guid LineId { get; init; }

    public required int Quantity { get; init; }

    public required decimal UnitPrice { get; init; }

    public static OrderLine Random => new()
    {
        LineId = Guid.NewGuid(),
        Quantity = RandomHelper.GenInt(),
        UnitPrice = RandomHelper.GenDecimal()
    };

}