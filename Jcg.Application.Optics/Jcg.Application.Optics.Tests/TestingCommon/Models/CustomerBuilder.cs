namespace Jcg.Application.Optics.Tests.TestingCommon;

public class CustomerBuilder
{
    private Customer _customer = Customer.Random;
    
    public CustomerBuilder AddOrder(out Order order)
    {
        order = Order.Random;
        _customer = _customer with
        {
            Orders = _customer.Orders.Append(order).ToArray()
        };
        return this;
    }

    public CustomerBuilder AddOrderLine(Order order, out OrderLine line)
    {
        line = OrderLine.Random;
        order = order with
        {
            Lines = order.Lines.Append(line).ToArray()
        };
        _customer = _customer with
        {
            Orders = _customer.Orders.Select(o =>
                o.OrderId == order.OrderId ? order : o).ToArray()
        };
        return this;
    }

    public Customer Build()
    {
        return _customer;
    }
}