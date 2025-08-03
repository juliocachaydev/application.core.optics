namespace Jcg.Application.Core.Optics.Tests.TestCommon.Models;

public class CustomerBuilder
{
    private Customer _customer = Customer.Random;
    
    public CustomerBuilder AddOrder(out Order order)
    {
        order = Order.Random;
        _customer = _customer with { Orders = _customer.Orders.Append(order).ToArray() };
        return this;
    }

    public CustomerBuilder AddOrderItem(Order order, out OrderItem item)
    {
        item = OrderItem.Random;
        order = order with { Items = order.Items.Append(item).ToArray() };
        _customer = _customer with
        {
            Orders = _customer.Orders.Select(o => o.OrderId == order.OrderId ? order : o).ToArray()
        };
        return this;
    }

    public Customer Build()
    {
        return _customer with { };
    }
}