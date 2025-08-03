using Jcg.Application.Core.Optics.Tests.TestCommon.Models;

namespace Jcg.Application.Core.Optics.Tests;

public class LensTests
{
    [Fact]
    public void CanOperateOnProperty()
    {
        // ***** ARRANGE *****

        var customer = Customer.Random;

        var customerNameLens = customer.CreateLens(
            cust => cust.CustomerName,
            (cust, name) => cust with { CustomerName = name });

        // ***** ACT *****

        customerNameLens.Value = "George";

        // ***** ASSERT *****
        
        Assert.Equal("George", customerNameLens.RootValue.CustomerName);
        
        Assert.Equal("George", customerNameLens.Value);

    }

    [Fact]
    public void CanOperateOnNestedProperty()
    {
        // ***** ARRANGE *****

        var customer = Customer.Random;

        var customerContactAddressStreetLens = customer
            .CreateLens(cust => cust.ContactInfo,
                (cust, contactInf) => cust with { ContactInfo = contactInf })
            .FocusLens(contactInfo => contactInfo.Address,
                (contactInf, addr) => contactInf with { Address = addr })
            .FocusLens(address => address.Street,
                (add, street) => add with { Street = street });

        // ***** ACT *****

        customerContactAddressStreetLens.Value = "Elm Street";

        // ***** ASSERT *****
        
        Assert.Equal("Elm Street", customerContactAddressStreetLens.RootValue.ContactInfo.Address.Street);
        Assert.Equal("Elm Street", customerContactAddressStreetLens.Value);

    }

    [Fact]
    public void CanAddItemToNestedCollection()
    {
        // ***** ARRANGE *****

        var customer = new CustomerBuilder()
            .AddOrder(out var order1)
            .AddOrder(out var order2)
            .Build();

        var customerOrdersLens = customer
            .CreateLens(cust => cust.Orders,
                (cust, orders) => cust with { Orders = orders });

        // ***** ACT *****

        customerOrdersLens.AddWhenDoesNotExists(order => order.Number == 11111,
            () => new Order
            {
                OrderId = Guid.NewGuid(),
                Number = 11111,
                Items = []
            });
        
        // ***** ASSERT *****
        
        Assert.Equal(3, customerOrdersLens.RootValue.Orders.Count());
        Assert.Contains(customerOrdersLens.RootValue.Orders, o => o.Number == 11111);

    }

    [Fact]
    public void CanRemoveItemFromNestedCollection()
    {
        // ***** ARRANGE *****

        var customer = new CustomerBuilder()
            .AddOrder(out var order1)
            .AddOrder(out var order2)
            .Build();

        var customerOrdersLens = customer
            .CreateLens(cust => cust.Orders,
                (cust, orders) => cust with { Orders = orders });

        // ***** ACT *****

        customerOrdersLens.RemoveWhenExists(o => o.OrderId == order1.OrderId);
        
        // ***** ASSERT *****
        
        Assert.Single(customerOrdersLens.RootValue.Orders);
        Assert.Contains(customerOrdersLens.RootValue.Orders, o => o.OrderId == order2.OrderId);

    }
    
    [Fact]
    public void CanUpdateItemFromNestedCollection()
    {
        // ***** ARRANGE *****

        var customer = new CustomerBuilder()
            .AddOrder(out var order1)
            .AddOrder(out var order2)
            .Build();

        var customerOrdersLens = customer
            .CreateLens(cust => cust.Orders,
                (cust, orders) => cust with { Orders = orders });

        // ***** ACT *****

        customerOrdersLens.UpdateWhenExists(order => order.OrderId == order2.OrderId,
            order => order with { Number = 12345});
        
        // ***** ASSERT *****
        
        Assert.Equal(12345, customerOrdersLens.RootValue.Orders.First(o=> o.OrderId == order2.OrderId).Number);

    }

    [Fact]
    public void CanAddItemFromDeeplyNestedCollection()
    {
        // ***** ARRANGE *****

        var customer = new CustomerBuilder()
            .AddOrder(out var order1)
            .AddOrderItem(order1, out var line1)
            .Build();

        var customerOrderItemsLens = customer
            .CreateLens(
                c => c.Orders,
                (c, ord) => c with { Orders = ord })
            .FocusLens(orders => orders.First(o => o.OrderId == order1.OrderId),
                (orders, ord) => orders = orders.Select(o =>
                    o.OrderId == order1.OrderId ? ord : o))
            .FocusLens(orderOne => orderOne.Items,
                (orderO, it) => orderO with { Items = it });

        // ***** ACT *****
        
        customerOrderItemsLens.AddWhenDoesNotExists(item => item.ProductName == "Bolts",
            () => new OrderItem
            {
                ProductName = "Bolts",
                Quantity = 100,
                Price = 10m
            });

        // ***** ASSERT *****
        
        var resultingOrder = customerOrderItemsLens.RootValue.Orders.First(o => o.OrderId == order1.OrderId);
        
        Assert.Equal(2, resultingOrder.Items.Count());
        Assert.Contains(resultingOrder.Items, i => i.ProductName == "Bolts");

    }
    
    [Fact]
    public void CanRemoveItemFromDeeplyNestedCollection()
    {
        // ***** ARRANGE *****

        var customer = new CustomerBuilder()
            .AddOrder(out var order1)
            .AddOrderItem(order1, out var line1)
            .Build();

        var customerOrderItemsLens = customer
            .CreateLens(
                c => c.Orders,
                (c, ord) => c with { Orders = ord })
            .FocusLens(orders => orders.First(o => o.OrderId == order1.OrderId),
                (orders, ord) => orders = orders.Select(o =>
                    o.OrderId == order1.OrderId ? ord : o))
            .FocusLens(orderOne => orderOne.Items,
                (orderO, it) => orderO with { Items = it });

        // ***** ACT *****
        
        customerOrderItemsLens.RemoveWhenExists(item => item.ProductName == line1.ProductName);

        // ***** ASSERT *****
        
        var resultingOrder = customerOrderItemsLens.RootValue.Orders.First(o => o.OrderId == order1.OrderId);
        
        Assert.Empty(resultingOrder.Items);

    }
    
    [Fact]
    public void CanUpdateItemFromDeeplyNestedCollection()
    {
        // ***** ARRANGE *****

        var customer = new CustomerBuilder()
            .AddOrder(out var order1)
            .AddOrderItem(order1, out var line1)
            .Build();

        var customerOrderItemsLens = customer
            .CreateLens(
                c => c.Orders,
                (c, ord) => c with { Orders = ord })
            .FocusLens(orders => orders.First(o => o.OrderId == order1.OrderId),
                (orders, ord) => orders = orders.Select(o =>
                    o.OrderId == order1.OrderId ? ord : o))
            .FocusLens(orderOne => orderOne.Items,
                (orderO, it) => orderO with { Items = it });

        // ***** ACT *****
        
        customerOrderItemsLens.UpdateWhenExists(item => item.ProductName == line1.ProductName,
            item => item with { ProductName = "Bolts"});

        // ***** ASSERT *****
        
        var resultingOrder = customerOrderItemsLens.RootValue.Orders.First(o => o.OrderId == order1.OrderId);
        
        Assert.Single(resultingOrder.Items);
        Assert.Contains(resultingOrder.Items, i => i.ProductName == "Bolts");

    }
}