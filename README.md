# Overview
A C# Implementation of Functional Lenses, but adapted to Object-Oriented Programming.
## License
MIT
## Dependencies
⦁   Net Standard 2.1
## Motivation
I always find complexity when updating nested objects, especially when dealing with nested collections. Common OOP Patterns like the Builder Pattern and some
other tricks work, but they are not good enough because:
1. They cause coupling
2. Writing builders consume time.

Manipulating data structures can be challenging, but Functional Programming offers an elegant solution through the use of Lenses. A Lens enables you to focus on a specific part of a data structure, allowing you to get or set values without having to consider the entire structure.

This library applies some of those concepts to the object-oriented programming world, allowing you to update objects with a lens that is generic and composable.

It also works great with immutable records, as demonstrated in the tests.

You can:
1. Update an object property.
2. Update a nested object property.
3. Update a collection of objects.
4. Update a nested collection of objects.
5. Update a property on an object inside a nested collection

There is no limit to the depth of the nested objects.

## Example
We will use the following [Customer class](https://github.com/juliocachaydev/application.core.optics/blob/main/Jcg.Application.Core.Optics/Jcg.Application.Core.Optics.Tests/TestCommon/Models/Customer.cs) for the examples:
```json 
{
  "CustomerId": "b1a2c3d4-e5f6-7890-abcd-1234567890ef",
  "CustomerName": "Customer_b1a2c3d4e5f67890abcd1234567890ef",
  "ContactInfo": {
    "Address": {
      "Street": "Market Street"
    }
  },
  "Orders": [
    {
      "OrderId": "c0ffee12-3456-7890-abcd-1234567890ab",
      "Number": 11111,
      "Items": [
        {
          "ProductName": "Bolts",
          "Quantity": 100,
          "Price": 10.0
        }
      ]
    }
  ]
}
```

We will also use a [Customer Builder](https://github.com/juliocachaydev/application.core.optics/blob/main/Jcg.Application.Core.Optics/Jcg.Application.Core.Optics.Tests/TestCommon/Models/CustomerBuilder.cs) to facilitate configuring the Customer object for the examples.


### Example 1: Set the Customer.ContactInfo.Address.Street to "Main Street"
The following is a simple example to show the basic functionality.

```csharp
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

        // The updated object is always available in the RootObject property
        Assert.Equal("Elm Street", customerContactAddressStreetLens.RootObject.ContactInfo.Address.Street);
        Assert.Equal("Elm Street", customerContactAddressStreetLens. Value);
    }
```

### Example 2: Add a new Order to the Customer
Here, a more complex use case. Adding an order to the Customer.Orders collection.

Keep in mind that we are not mutating the original object, but creating a new one with the updated collection.
```csharp
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

        Assert.Equal(3, customerOrdersLens.RootObject.Orders.Count());
        Assert.Contains(customerOrdersLens.RootObject.Orders, o => o.Number == 11111);
    }
```

### Example 3: Focus on a deeply nested collection and update a particular object property value
In this example, our Customer has an order that includes an item. We will update the item's product name.

```csharp 
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
            item => item with { ProductName = "Bolts" });

        // ***** ASSERT *****

        var resultingOrder = customerOrderItemsLens.RootObject.Orders.First(o => o.OrderId == order1.OrderId);

        Assert.Single(resultingOrder.Items);
        Assert.Contains(resultingOrder.Items, i => i.ProductName == "Bolts");
    }
```

### Other Examples
There are more use cases, you can see [in the following test class](https://github.com/juliocachaydev/application.core.optics/blob/main/Jcg.Application.Core.Optics/Jcg.Application.Core.Optics.Tests/LensTests.cs)

For instance:
- Updating and reading properties.
- Adding items to a deeply nested collection.
- Removing items from a deeply nested collection.
- Updating a particular item from a deeply nested collection.


## Credits
Author: Julio C. Cachay. Chattanooga, TN, USA.

This library is inspired by the concept of lenses in functional programming, and in the [optics.ts](https://akheron.github.io/optics-ts/) library
