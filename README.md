# Overview

A C# Implementation of Functional Lenses, but adapted to Object-Oriented Programming.

## License

MIT

## Dependencies

⦁ Net Standard 2.1

## Motivation

I always find complexity when updating nested objects, especially when dealing with nested collections. Common OOP
Patterns like the Builder Pattern and some
other tricks work, but they are not good enough because:

1. They cause coupling
2. Writing builders consume time.

Manipulating data structures can be challenging, but Functional Programming offers an elegant solution through the use
of Lenses. A Lens enables you to focus on a specific part of a data structure, allowing you to get or set values without
having to consider the entire structure.

This library applies some of those concepts to the object-oriented programming world, allowing you to update objects
with a lens that is generic and composable.

It also works great with immutable records, as demonstrated in the tests.

You can:

1. Update an object property.
2. Update a nested object property.
3. Update a collection of objects.
4. Update a nested collection of objects.
5. Update a property on an object inside a nested collection

There is no limit to the depth of the nested objects.

## Examples 
### Basic Example (single lens, no nesting)


```csharp
Customer customer = new CustomerBuilder().Build();

// Customer is a record so we can use non-destructive mutation to create a new
// instance with a different name
customer = customer with
{
    CustomerName = "Tom"
};

// {
//   "CustomerId": "f7a9b2c1-4d3e-4a5b-8c2d-1e2f3a4b5c6",
//   "CustomerName": "Tom",
//   "ContactInfo": {...}
// }

// Using named parameters for clarity
ILens<Customer, string> customerNameLens = customer.CreateLens(
    getter: c => c.CustomerName,
    setter: (c, name) => c with { CustomerName = name }
);

// Initially, the Lens.RootObject is the customer instance.
Assert.Same(customer, customerNameLens.RootObject);

// You can set the name using the value property, like it was a simple {get; set;} property.
customerNameLens.Value = "George";

// Under the hood, this creates a new instance of the customer available through the Lens.RootObject.
Assert.NotSame(customer, customerNameLens.RootObject);

// As expected, that new instance has the updated name.
Assert.Equal("George", customerNameLens.Value);
```

### Example 2: Nested lens focusing on a deeply nested property 

```csharp 
Customer customerObject = new CustomerBuilder()
            .Build();
        
        // Customer is a record so we can use non-destructive mutation to create a new
        // instance with a different name
        customerObject = customerObject with
        {
            ContactInfo = customerObject.ContactInfo with
            {
                Address = customerObject.ContactInfo.Address with
                {
                    Street = "Market Street"
                }
            }
        };
        
        // {
        //   "CustomerId": "...",
        //   "CustomerName": "...",
        //   "ContactInfo": {
        //     "Address": {
        //       "Street": "Market Street" <-- we will update this
        //     }
        //   }
        // }
        
        // Using named parameters for clarity
        ILens<Customer, string> customerContactAddressStreetLens =
            customerObject
                // Create a lens that focuses on the ContactInfo property
                .CreateLens(
                    getter: customer => customer.ContactInfo,
                    setter: (customer, contactInfo) => customer with { ContactInfo = contactInfo }
                )
                // Compose the lens to focus on the ContactInfo.Address property
                .FocusLens(
                    getter: contactInfo => contactInfo.Address,
                    setter: (contactInfo, address) => contactInfo with { Address = address }
                )
                // Compose the lens to focus on the Address.Street property
                .FocusLens(
                    getter: address => address.Street,
                    setter: (address, street) => address with { Street = street }
                );
        
        // Lets change the street name using the lens
        customerContactAddressStreetLens.Value = "Elm Street";
        
        // Now, the lens.RootObject has the changed value
        Assert.Equal("Elm Street", customerContactAddressStreetLens.RootObject.ContactInfo.Address.Street);
        
        // Keep in mind the original object was not modified
        Assert.Equal("Market Street", customerObject.ContactInfo.Address.Street);
```

### Example 3: Nested lens focusing on a collection of objects

```csharp
Customer customerObject = new CustomerBuilder()
    .AddOrder(out var order1)
    .AddOrderItem(order1, out var orderItem1)
    .Build();

// {
//   "CustomerId": "...",
//   "CustomerName": "...",
//   "ContactInfo": { ... },
//   "Orders": [
//     {
//       "OrderId": "o1a2b3c4-...",
//       "OrderDate": "2024-06-10T00:00:00Z",
//       "Items": [
//         {
//           "ProductName": "Sample Product",
//           "Quantity": 1,      // <-- We will update this
//           "UnitPrice": 9.99   // <-- We will also update this
//         }
//       ]
//     }
//   ]
// }

// Focuses on Order1
var customerOrder1Lens = customerObject.CreateLens(
    getter: customer => customer.Orders.First(o => o.OrderId == order1.OrderId),
    setter: (customer, order) => customer with
    {
        Orders = customer.Orders.Select(o => o.OrderId == order.OrderId ? order : o)
    }
);

// Focuses on Order1.Item1
var order1Item1Lens = customerOrder1Lens.FocusLens(
    getter: order => order.Items.First(i => i.ProductName == orderItem1.ProductName),
    setter: (order, orderItem) => order with
    {
        Items = order.Items.Select(i => i.ProductName == orderItem.ProductName ? orderItem : i)
    }
);

// Compose lenses to focus on multiple properties
var order1Item1QuantityLens = order1Item1Lens.FocusLens(
    getter: item => item.Quantity,
    setter: (item, quantity) => item with { Quantity = quantity }
);

var order1Item1UnitPriceLens = order1Item1Lens.FocusLens(
    getter: item => item.Price,
    setter: (item, price) => item with { Price = price }
);

// Update the quantity to 300
order1Item1QuantityLens.Value = 300;

// Update the price to 99.99
order1Item1UnitPriceLens.Value = 99.99m;

var resultingItem = customerOrder1Lens.RootObject
    .Orders.First(o => o.OrderId == order1.OrderId)
    .Items.First(i => i.ProductName == orderItem1.ProductName);

Assert.Equal(300, resultingItem.Quantity);
Assert.Equal(99.99m, resultingItem.Price);
```

### Other Examples

You can leverage a lens that focuses on a collection to modify that collection.

There are more use cases, you can
see [in the following test class](https://github.com/juliocachaydev/application.core.optics/blob/main/Jcg.Application.Core.Optics/Jcg.Application.Core.Optics.Tests/LensTests.cs)

For instance:

- Updating and reading properties.
- Adding items to a deeply nested collection.
- Removing items from a deeply nested collection.
- Updating a particular item from a deeply nested collection.

## Credits

Author: Julio C. Cachay. Chattanooga, TN, USA.

This library is inspired by the concept of lenses in functional programming, and in
the [optics.ts](https://akheron.github.io/optics-ts/) library
