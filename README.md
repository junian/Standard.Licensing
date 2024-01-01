[![NuGet](https://img.shields.io/nuget/v/Standard.Licensing.svg)](https://www.nuget.org/packages/Standard.Licensing/)
[![NuGet](https://img.shields.io/nuget/dt/Standard.Licensing.svg)](https://www.nuget.org/packages/Standard.Licensing/)
[![Build status](https://ci.appveyor.com/api/projects/status/8jjw5v89h83p80ax/branch/master?svg=true)](https://ci.appveyor.com/project/junian/standard-licensing/branch/master)

## About

Easy-to-use licensing library for .NET Framework, Mono, .NET Core, and Xamarin products.

This is project is derived from [Portable.Licensing](https://github.com/dnauck/Portable.Licensing/) library. The purpose of this fork is to add support for more .NET platforms, especially .NET Standard and .NET Core.

![](https://1.bp.blogspot.com/-yv8fTM-7QdQ/WoghR2w1KJI/AAAAAAAAC28/0BGyFsS2nGoN9t2QnoXgMbiWVUwUipH1gCLcBGAs/s1600/standard-licensing-logo.png)

## Installation

Get [Standard.Licensing](https://www.nuget.org/packages/Standard.Licensing/) from NuGet.

```powershell
PM> Install-Package Standard.Licensing
```

## Usage

### Create a private and public key for your product

Standard.Licensing uses the Elliptic Curve Digital Signature Algorithm (ECDSA) to ensure the license cannot be altered after creation.

First you need to create a new public/private key pair for your product:

```csharp
var keyGenerator = Standard.Licensing.Security.Cryptography.KeyGenerator.Create(); 
var keyPair = keyGenerator.GenerateKeyPair(); 
var privateKey = keyPair.ToEncryptedPrivateKeyString(passPhrase);  
var publicKey = keyPair.ToPublicKeyString();
```

Store the private key securely and distribute the public key with your product.
Normally you create one key pair for each product, otherwise it is possible to use a license with all products using the same key pair.
If you want your customer to buy a new license on each major release you can create a key pair for each release and product.

### Create the license generator

Now we need something to generate licenses. This could be easily done with the `LicenseFactory`:

```csharp
var license = License.New()  
    .WithUniqueIdentifier(Guid.NewGuid())  
    .As(LicenseType.Trial)  
    .ExpiresAt(DateTime.Now.AddDays(30))  
    .WithMaximumUtilization(5)  
    .WithProductFeatures(new Dictionary<string, string>  
        {  
            {"Sales Module", "yes"},  
            {"Purchase Module", "yes"},  
            {"Maximum Transactions", "10000"}  
        })  
    .LicensedTo("John Doe", "john.doe@example.com")  
    .CreateAndSignWithPrivateKey(privateKey, passPhrase);
```

Now you can take the license and save it to a file:

```csharp
File.WriteAllText("License.lic", license.ToString(), Encoding.UTF8);
```

or

```csharp
license.Save(xmlWriter);
```

### Validate the license in your application ###

The easiest way to assert the license is in the entry point of your application.

First load the license from a file or resource:

```csharp
var license = License.Load(...);
```

Then you can assert the license:

```csharp
using Standard.Licensing.Validation;

var validationFailures = license.Validate()  
                                .ExpirationDate()  
                                .When(lic => lic.Type == LicenseType.Trial)  
                                .And()  
                                .Signature(publicKey)  
                                .AssertValidLicense();
```

Standard.Licensing will not throw any Exception and just return an enumeration of validation failures.

Now you can iterate over possible validation failures:

```csharp
foreach (var failure in validationFailures)
     Console.WriteLine(failure.GetType().Name + ": " + failure.Message + " - " + failure.HowToResolve);
```

Or simply check if there is any failure:

```csharp
if (validationFailures.Any())
   // ...
```

Make sure to call `validationFailures.ToList()` or `validationFailures.ToArray()` before using the result multiple times.

## Credits

- [dnauck/Portable.Licensing](https://github.com/dnauck/Portable.Licensing/) for the original work.

## License

This project is licensed under [MIT License](https://github.com/junian/Standard.Licensing/blob/master/LICENSE).
