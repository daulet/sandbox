# Echo

Swiss knife for your .NET dependencies

[![Build status](https://img.shields.io/appveyor/ci/daulet/echo/master.svg)](https://ci.appveyor.com/project/daulet/echo)
[![Coveralls status](https://coveralls.io/repos/github/daulet/Echo/badge.svg)](https://coveralls.io/github/daulet/Echo)
[![Codecov status](https://img.shields.io/codecov/c/github/daulet/echo.svg)](https://codecov.io/gh/daulet/Echo)
[![Nuget](https://img.shields.io/nuget/v/Echo.svg)](https://www.nuget.org/packages/echo/)

## Restrict sensitive operations

Mark methods and properties that you'd like to stub out at runtime:

``` csharp
public interface IBilling
{
    [Restricted]
    bool Charge(string username, int amount);

    DateTimeOffset GetExpirationDate(string username);

    int GetPrice(RegionInfo region);
}
```

Use the sensitive dependency in your test or dry run without actually worrying about making sensitive or expensive calls (e.g. Charge() in this example), while using the real implementation of the rest of the methods:

``` csharp
var provider = new RestrictingProvider(logger);

// get instance of the real dependency
var resource = new CreditCardBilling(logger);

// restrict methods that you don't want call in dry run
var restrictedResource = provider.GetRestrictingTarget<IBilling>(resource);

// inject restricted dependency to the implementation under test
var implementation = new SubscriptionImplementation(restrictedResource);

// dry run your implementation
implementation.Renew("cartman", RegionInfo.CurrentRegion);
```

Which will generate the following output:

``` text
Getting expiration for cartman
Obtaining price for United States
Restricting call to Charge with cartman, 100
```

See [RestrictionSample](.\sample\RestrictionSample) for the full sample.
