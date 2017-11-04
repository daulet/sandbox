# Echo

Swiss knife for your .NET dependencies

[![Build status](https://img.shields.io/appveyor/ci/daulet/echo/master.svg)](https://ci.appveyor.com/project/daulet/echo)
[![Codecov status](https://img.shields.io/codecov/c/github/daulet/echo.svg)](https://codecov.io/gh/daulet/Echo)
[![Nuget](https://img.shields.io/nuget/v/Echo.svg)](https://www.nuget.org/packages/echo/)

## Restrict sensitive operations

This demonstrates how you can run an integrated test with *real* dependencies in a semi-sandboxed manner: mark sensitive operations as restricted and test your code with confidence that it won't affect sensitive state.

Say you'd like to test your code (below) with the real dependency (_billing):

``` csharp
var expirationDate = _billing.GetExpirationDate(username);

if (expirationDate <= DateTimeOffset.UtcNow)
{
    var price = _billing.GetPrice(region);
    return _billing.Charge(username, price);
}

return true;
```

The problem is that you don't really want to call Charge() during your test while you still want to be able to exercise your integration with other methods of _billing (instance of IBilling). You can mark such methods as [Restricted]:

``` csharp
public interface IBilling
{
    [Restricted]
    bool Charge(string username, int amount);

    DateTimeOffset GetExpirationDate(string username);

    int GetPrice(RegionInfo region);
}
```

Then you can use RestrictingProvider to wrap your real dependency and mock out restricted methods:

``` csharp
var provider = new RestrictingProvider(logger);

// get instance of the real dependency
var resource = new CreditCardBilling(logger);

// restrict methods that you don't want call in dry run
var restrictedResource = provider.GetRestrictedTarget<IBilling>(resource);

// inject restricted dependency to the implementation under test
var implementation = new SubscriptionImplementation(restrictedResource);

// dry run your implementation
implementation.Renew("cartman", RegionInfo.CurrentRegion);
```

Now you don't have to worry about your code accidentally calling real implementation of Charge(). Here is output for this example:

``` text
Getting expiration for cartman
Obtaining price for United States
Restricting call to Charge with cartman, 100
```

See [RestrictionSample](/sample/RestrictionSample) for the full example.

A common use case for this feature is testing your state changing logic. Imagine that you'd like to update a subset of records in storage. If you mark write operations as restricted, while using real (production) storage adapter, you'd be able to dry run, in other words test, your implementation and confirm, for example, number of records that will be updated. The beauty of this approach is that you don't have to modify your code to enable test functionality, and when the time comes to run the script for real, simply inject the full (not restricted) implementation of your storage adapter.
