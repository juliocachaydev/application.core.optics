using Jcg.Application.Optics.Api;
using Jcg.Application.Optics.Tests.TestingCommon;

namespace Jcg.Application.Optics.Tests.Api;

public class RemovableLensesTests
{
    [Fact]
    public void CanRemoveChildren()
    {
        // ***** ARRANGE *****

        var customer = new CustomerBuilder()
            .AddOrder(out var order1)
            .AddOrder(out var order2)
            .AddOrder(out var order3)
            .Build();

        var customerOrdersLens = customer.CreateCollectionLens(
            d => d.Orders,
            (cust, orders) => cust with { Orders = orders });

        var removableOrdersLens = customerOrdersLens.CreateRemovableLens(o => o.OrderId == order2.OrderId);

        // ***** ACT *****
        
        removableOrdersLens.Remove();
        

        // ***** ASSERT *****
        
        Assert.DoesNotContain(removableOrdersLens.MutatedData.Orders, o =>
            o == order2);
        
        Assert.Equal(2, removableOrdersLens.MutatedData.Orders.Count());
        Assert.Contains(removableOrdersLens.MutatedData.Orders, o =>
            o == order1);
        Assert.Contains(removableOrdersLens.MutatedData.Orders, o =>
            o == order3);
    }

    [Fact]
    public void CanRemoveGrandChildren()
    {
        // ***** ARRANGE *****

        var customer = new CustomerBuilder()
            .AddOrder(out var order1)
            .AddOrderLine(order1, out var line1)
            .AddOrderLine(order1, out var line2)
            .Build();

        var customerOrdersLens = customer.CreateCollectionLens(
            c => c.Orders,
            (cust, orders) => cust with { Orders = orders });
        

        var customerOrdersLinesLens = customerOrdersLens
            .FocusCollection(orders => orders,
                (d, v) => d with { Orders = v });)

        // ***** ACT *****
        
        removableOrdersLens.Remove();
        

        // ***** ASSERT *****
        
        Assert.DoesNotContain(removableOrdersLens.MutatedData.Orders, o =>
            o == order2);
        
        Assert.Equal(2, removableOrdersLens.MutatedData.Orders.Count());
        Assert.Contains(removableOrdersLens.MutatedData.Orders, o =>
            o == order1);
        Assert.Contains(removableOrdersLens.MutatedData.Orders, o =>
            o == order3);
    }
}