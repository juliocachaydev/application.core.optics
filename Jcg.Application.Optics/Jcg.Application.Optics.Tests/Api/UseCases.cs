using Jcg.Application.Optics.Api;
using Jcg.Application.Optics.Tests.TestingCommon;

namespace Jcg.Application.Optics.Tests.Api;

public class UseCases
{
    [Fact]
    public void SimpleProperty()
    {
        // ***** ARRANGE *****

        var customer = Customer.Random;
        
        var nameLens = customer.CreateLens(
            cust => cust.Name,
            (cust,  name) => cust with { Name = name });

        // ***** ACT *****

        nameLens.Value = "George";
        
        
        // ***** ASSERT *****
        
        Assert.Equal("George", nameLens.Value);
        Assert.Equal("George", nameLens.UpdatedRoot.Name);
    }
    
    [Fact]
    public void NestedProperty()
    {
        // ***** ARRANGE *****

        var customer = Customer.Random;
        
        var contactInfoLens = customer.CreateLens(
            cust => cust.ContactInfo,
            (cust,  contactInf) => cust with { ContactInfo = contactInf });
        
        var contactInfoPhoneLens = contactInfoLens.FocusLens(
            contactInf => contactInf.Phone,
            (contactInf, phon) => contactInf with { Phone = phon });

        // ***** ACT *****

        contactInfoPhoneLens.Value = "(423) 434-5252";
        
        // ***** ASSERT *****
        
        Assert.Equal("(423) 434-5252", contactInfoPhoneLens.UpdatedRoot.ContactInfo.Phone);
    }

    [Fact]
    public void ManipulateCollectionWithLens()
    {
        // ***** ARRANGE *****

        var customer = new CustomerBuilder()
            .AddOrder(out var order1)
            .AddOrder(out var order2)
            .Build();

        var customerOrdersLens = customer.CreateLens(
            cust => cust.Orders,
            (cust, orders) => cust with { Orders = orders });

        // ***** ACT *****\

        customerOrdersLens.Value = customerOrdersLens.UpdatedRoot.Orders.Where(o => o.OrderId == order1.OrderId).ToArray();
        
        // ***** ASSERT *****
        
        Assert.Single(customerOrdersLens.UpdatedRoot.Orders);
        Assert.Contains(customerOrdersLens.UpdatedRoot.Orders, o => o == order2);
    }
}