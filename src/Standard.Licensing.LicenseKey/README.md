## Usage

### Creating License Signing Parameters

StandardLicensing License Keys uses .Net's Cryptology APIs to create and sign the license keys it generates, more specifically, the `RsaSsaPssSha256` algorithm. Keys can be any length in bits that the RSA algorithm permits (512, 1024, 3096, etc...). The default is 2048 bits.

#### Generating New License Parameters

You can generate new keys for license signing by instantiating the `LicenseSigningParameters` with no arguments or one argument representing your desired key size.

```csharp
// optionally you can pass the key size, defaults to 2048 bits
using var licenseParameters = new LicenseSigningParameters();
```

#### Saving License Parameters

If you wish to store your public key in your application or simply wish to use the same private key for every new license you generate you will have to save your license parameters. This can be accomplished easily with the `Export` and `Import` methods which also support asynchronous code.

```csharp
licenseParameters.Export("keys.txt");
// then, when you need the keys again
using var signingParameters = LicenseSigningParameters.Import("keys.txt");
```

### Creating the License Generator

StandardLicensing License Keys supports a base set of fields for licenses, but also has functionality to support further configuration through generics. This allows you to specify further strongly typed license details in the `KeyData` property.

```Csharp
public class TestLicense 
{
    public int MaxUsers { get; set; }

    public string IssuedTo { get; set; }
}
```

#### Using the Fluent API

StandardLicensing License Keys also supports a fluent API to configure all of its license properties.

```CSharp
using var signingParameters = new LicenseSigningParameters();

var licenseInformation = new TestLicense
{
    MaxUsers = 10000,
    IssuedTo = "Some Random User"
};

var signedLicense = LicenseKeyManager.Create(licenseInformation)
    .WithName("Test License")
    .WithType(LicenseType.Enterprise)
    .WithActivationDate(DateTime.UtcNow.AddDays(1))
    .WithExpirationDate(DateTime.UtcNow.AddDays(30))
    .CreateAndSignLicense(signingParameters);
```

#### Using Standard Assignments

If you do not want to use the fluent API for creation you can call `CreateLicense` on the `ILicenseFactory` interface and manipulate the license with standard assignments, then sign the license later on with the `Sign` method. Example:

```csharp
using var signingParameters = new LicenseSigningParameters();

var licenseInformation = new TestLicense
{
    MaxUsers = 10000,
    IssuedTo = "Some Random User"
};

var license = LicenseKeyManager.Create(licenseInformation)
    .CreateLicense();

license.ExpirationDate = DateTime.UtcNow.AddDays(7);
license.LicenseType = LicenseType.Standard;
var signedLicense = license.Sign(signingParameters)
```

Once you have generated the license then you are free to transport it anywhere. So long as the public key is present the validator should be able to check its contents.

### Validate The License in Your Application

The license data can be stored however you please, however once you start to validate the license you must reconstruct the `SignedLicense` object that the license was originally exported as. This class only consists of two fields, `PublicKey` and `LienseData` so this is not hard to do.

```csharp
var licenseData = GetLicenseData();
var publicKey = GetLicenseKey();

var signedLicense = new SignedLicense(licenseData, publicKey);
```

once you have the `SignedLicense` object, you can use the license validation API to validate your license.

```Csharp

var signedLicense = GetSignedLicense();

if(!signedLicense.SignatureValid())
    throw new Exception("This license has been tampered with!");

var validationResults = LicenseKeyManager.Load<TestLicense>(signedLicense)
    .TypeNot(LicenseType.Trial)
    .ActivatesBefore(DateTime.UtcNow)
    .ExpiresAfter(DateTime.UtcNow, "License is expired")
    .MeetsCondition(license => license.LicenseData.IssuedTo == "Some Random User",
        "This license was issued to Some Random User, and you're not them!")
    .IfExpiresBefore(DateTime.UtcNow.AddDays(7), expirationDate 
        => Console.WriteLine("warning, license expires soon")
    .Validate();

if(validationResults.HasErrors)
    foreach(var error in validationResults.Errors)
        Console.WriteLine(error);
```

StandardLicensing License Keys will not throw any exceptions unless it fails to authenticate the license with its public key.

All errors will be put in the `Errors` property on the `LicenseValidationResults<T>` class and the user will be able to enumerate them manually. This class also exposes the physical license in the `LicenseKey` property, so license values can be read after validation.