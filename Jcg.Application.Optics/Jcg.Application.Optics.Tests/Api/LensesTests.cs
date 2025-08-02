using Jcg.Application.Optics.Api;
using Jcg.Application.Optics.Tests.TestingCommon;

namespace Jcg.Application.Optics.Tests.Api;

public class LensesTests
{
    [Fact]
    public void CanGetAndSetRootProperty()
    {
        // ***** ARRANGE *****

        var data = Customer.Random;

        var customerNameLens = data.CreateLens(x => x.Name,
            (d, v) => d with { Name = v });

        // ***** ACT *****

        customerNameLens.Value = "George";

        // ***** ASSERT *****
        
        // Keeps track of the initial data
        Assert.Same(data, customerNameLens.InitialData);
        
        // Mutated data is updated with the new value
        var expected = data with { Name = "George" };
        
        Assert.Equivalent(expected, customerNameLens.MutatedData);
        
        // getter
        Assert.Equal("George", customerNameLens.Value);
    }
    
    
    [Fact]
    public void CanFocusNestedProperty()
    {
        // ***** ARRANGE *****

        var data = Customer.Random;
        
        var customerContactInfoLens = data.CreateLens(x => x.ContactInfo,
            (d,v) => d with { ContactInfo = v });
        
        var customerContactInfoPhoneLens = customerContactInfoLens
            .FocusProperty(x => x.Phone, (d,v) => d with { Phone = v });

        // ***** ACT *****

        customerContactInfoPhoneLens.Value = "(706) 444-4444";

        // ***** ASSERT *****
        
        Assert.Equal("(706) 444-4444", customerContactInfoLens.MutatedData.ContactInfo.Phone);
    }
}