using Jcg.Application.Core.Optics.Tests.TestCommon.Models;

namespace Jcg.Application.Core.Optics.Tests;

public class Examples
{
    [Fact]
    public void Example1_SingleLens_NoNesting()
    {
        Customer customer = new CustomerBuilder()
            .Build();

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
       //  }

       // using named parameters for clarity
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
    }
    
    [Fact]
    public void MultipleLens_ForDeepProperty()
    {
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

        // using named parameters for clarity
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
        Assert.Equal("Elm Street", customerContactAddressStreetLens.RootObject
            .ContactInfo.Address.Street);
        
        // Keep in mind the original object was not modified
        Assert.Equal("Market Street", customerObject.ContactInfo.Address.Street);
    }

    [Fact]
    public void MultipleLenses_FocusOnNestedObjectInCollectionWithinAnotherCollection()
    {
         
        Customer customerObject = new CustomerBuilder()
            .AddOrder(out var order1)
            .AddOrderItem(order1, out var orderItem1)
            .Build();
        
        // {
        //   "CustomerId": "...",
        //   "CustomerName": "...",
        //   "ContactInfo": {
        //     ...
        //   },
        //   "Orders": [
        //     {
        //       "OrderId": "o1a2b3c4-...",
        //       "OrderDate": "2024-06-10T00:00:00Z",
        //       "Items": [
        //           "ProductName": "Sample Product", 
        //           "Quantity": 1, <-- We will update this
        //           "UnitPrice": 9.99 <-- We will also update this
        //         }
        //       ]
        //     }
        //   ]
        // }
        
        // focuses on Order1
        var customerOrder1Lens = customerObject.CreateLens(
            getter: customer => customer.Orders.First(o => o.OrderId == order1.OrderId),
            setter: (customer, order) => customer with
            {
                Orders = customer.Orders.Select(o => o.OrderId == order.OrderId ? order : o)
            });
        
        // focuses on Order1.Item1
        var order1Item1Lens = customerOrder1Lens.FocusLens(
            getter: order => order.Items.First(i => i.ProductName == orderItem1.ProductName),
            setter: (order, orderItem) => order with
            {
                Items = order.Items.Select(i => i.ProductName == orderItem.ProductName ? orderItem : i)
            });
        
        // See how we can use one lens to focus on multiple properties by composition 
        
        var order1Item1QuantityLens = order1Item1Lens.FocusLens(
            getter: item => item.Quantity,
            setter: (item, quantity) => item with { Quantity = quantity });
        
        var order1Item1UnitPriceLens = order1Item1Lens.FocusLens(
            getter: item => item.Price,
            setter: (item, price) => item with { Price = price });
        
        // Update the quantity to 300
        order1Item1QuantityLens.Value = 300;
        // Update the price to 99.99 
        order1Item1UnitPriceLens.Value = 99.99m;
        
        var resultingItem = customerOrder1Lens.RootObject
            .Orders.First(o => o.OrderId == order1.OrderId)
            .Items.First(i => i.ProductName == orderItem1.ProductName);
        
        Assert.Equal(300, resultingItem.Quantity);
        Assert.Equal(99.99m, resultingItem.Price);
    }

    [Fact]
    public void MultipleLenses_AddItemsToANestedCollection()
    {
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
        //       "OrderId": "...",
        //       "OrderDate": "...",
        //       "Items": [
        //         { "ProductName": "...", "Quantity": ..., "Price": ... }
        //         // We will add a new item here
        //       ]
        //     }
        //   ]
        // }
        
        // Focus on the Orders collection
        var customerOrdersLens = customerObject.CreateLens(
            getter: customer => customer.Orders,
            setter: (customer, orders) => customer with { Orders = orders }
        );
        
        // Focus on the specific order by OrderId
        var order1Lens = customerOrdersLens.FocusLens(
            getter: orders => orders.First(o => o.OrderId == order1.OrderId),
            setter: (orders, order) => orders.Select(o => o.OrderId == order.OrderId ? order : o)
        );
        
        // Focus on the Items collection of the order
        var order1ItemsLens = order1Lens.FocusLens(
            getter: order => order.Items,
            setter: (order, items) => order with { Items = items }
        );
        
        // This is an extension method that adds items to the collection 
        order1ItemsLens.AddWhenDoesNotExists(
            existsFunction: item => item.ProductName == "Bolts",
            factory: () => new OrderItem
            {
                ProductName = "Bolts",
                Quantity = 100,
                Price = 10m
            }
        );
        
        var resultingOrder = order1Lens.RootObject
            .Orders.First(o => o.OrderId == order1.OrderId);
        
        Assert.Equal(2, resultingOrder.Items.Count());
        Assert.Contains(resultingOrder.Items, i => i.ProductName == "Bolts");
        
    }
}