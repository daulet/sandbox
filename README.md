# Echo

Swiss knife for your .NET dependencies

[![Build status](https://img.shields.io/appveyor/ci/daulet/echo/master.svg)](https://ci.appveyor.com/project/daulet/echo)
[![Codecov status](https://img.shields.io/codecov/c/github/daulet/echo.svg)](https://codecov.io/gh/daulet/Echo)
[![Nuget](https://img.shields.io/nuget/v/Echo.svg)](https://www.nuget.org/packages/echo/)

## Restriction

Given Execute() method that throws but which is marked with [Restricted] attriubute in the interface as below:

``` csharp
public interface IDependency
{
    void Acknowledge();

    [Restricted]
    void Execute();
}

public class ThrowingDependency : IDependency
{
    public void Acknowledge()
    {
        Console.WriteLine($"Hello, I'm implementation of {GetType().Name}");
    }

    public void Execute()
    {
        throw new IOException();
    }
}
```

Using RestrictingProvider you can avoid calling undesired methods:

``` csharp
var restrictedObj = new RestrictingProvider(logger)
    .GetRestrictedTarget<IDependency>(
        new ThrowingDependency());

restrictedObj.Acknowledge();
restrictedObj.Execute();

Console.WriteLine("Reached end of execution");
```

The above will produce the following output: 

``` text
Hello, I'm implementation of ThrowingDependency
Reached end of execution
```

Note implementation of Acknowledge() is invoked, but not of Execute(). For more detailed example and interesting use cases [visit documentation](./docs/Restriction.md).

## License

[MIT](./LICENSE.md)