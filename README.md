# Nexus-Data-Store-Net
Nexus Data Store .NET is a .NET library for reading and modifying
data stored with [Nexus Data Store](https://github.com/TheNexusAvenger/Nexus-Data-Store)
on Roblox.

## Usage
Nexus Data Store .NET currently does *not* have a Nuget package and
must be added as a submodule. This may change in the future.

### API Key
The entry point for the class library is the `NexusDataStore` class.
In order to use it, it requires a game id (NOT place id) and Roblox
Open Cloud API key. [In the credentials dashboard](https://create.roblox.com/dashboard/credentials),
the API key must be created with the following:
- `DataStore - Create Datastore` *if the DataStore can be empty when setting values.*
- `DataStore - Create Entry` *if DataStore entries can be empty when setting values.*
- `DataStore - Read Entry`
- `DataStore - Update Entry` *if any calls that update values are called.*
- `Messaging Service - Publish` *if any calls that update values are called.*

### Loading Data Stores
While `NexusDataStore` can be constructed, it is recommended to
use `NexusDataStore.Get` to reduce duplicate objects being created.
With a `NexusDataStore` object, `GetDataStoreAsync` or `GetSaveDataByIdAsync`
can be used to get a `SaveData` object. Both will yield to load
data.

```cs
var gameId = 0; // Actual game id here.
var apiKey = "..."; // Roblox Open Cloud API key with the permissions required above.
var nexusDataStore = NexusDataStore.Get(gameId, apiKey); // Get a cached version of NexusDataStore for the game id. The API key is always updated in this regardless of if a cached version exists.

var saveData = await nexusDataStore.GetDataStoreAsync("MyDataStore", "MyKey"); // Similar to GetDataStore in NexusDataStore.
var saveData = nexusDataStore.GetDataStoreAsync("MyDataStore", "MyKey").Result; // Similar to GetDataStore in NexusDataStore.
var saveData = await nexusDataStore.GetSaveDataByIdAsync(12345); // Similar to GetSaveDataById and GetSaveData in NexusDataStore.
var saveData = nexusDataStore.GetSaveDataByIdAsync(12345).Result; // Similar to GetSaveDataById and GetSaveData in NexusDataStore.

saveData.Disconnect(); // Optional but recommended to remove cached SaveData.
```

`GetDataStoreAsync` and `GetSaveDataByIdAsync` will throw `OpenCloudResponseException`
if there is a problem loading. `OpenCloudUnauthorizedException` will
be used if the API key is invalid while `OpenCloudInsufficientScopeException`
will be used if the API key is valid but can't read the DataStore.
`OpenCloudResponseException` without a subclass in most cases will
be unexpected.

### Reading Data
Reading data is done using the `Get<T>` method. It will never yield or
throw exceptions besides casting. *Returned values are nullable. Make
sure to check the values before using them.*

```cs
var saveData = ...;

var someNumber = saveData.Get<long>();
var someString = saveData.Get<string>();
var someDictionary = saveData.Get<Dictionary<string, long>>();
var someCustomObject = saveData.Get<MyCustomClass>();
```

### Setting Data
Data can be changed using `SetAsync` and `UpdateAsync`. `SetAsync`
is the simpler of the options and does a single update for 1
key and value. Unlike Nexus Data Store, calls to `SetAsync` are
**not buffered**. Multiple back-to-back calls to `SetAsync` will
send requests for updates. `UpdateAsync` is *strongly* recommended
for multiple sets.

```cs
var saveData = ...;
await saveData.SetAsync("someKey", "someValue");
```

`UpdateAsync` is more complex and operates differently due to
limiations with variable length arguments in C#. Instead of
passing in a list of the original values and expecting a list
of new values to update (kept since PlayerDataStore for
interoperability), `UpdateAsync` takes in an `Action<ISaveData>`
and commits the changes together after the action completes.
Exceptions in the action will result in the save not completing.
**Do not pass in an `async` task as it will not wait for the action to complete.**

```cs
var saveData = ...;
await saveData.SetAsync("someKey", "someValue"); // Set up the example of how data is read in UpdateAsync.

await saveData.UpdateAsync(transactionSaveData => { // Do NOT use async transactionSaveData.
    transactionSaveData.Set("someKey", "someNewValue"); // Set exists in this context with ISaveData. SetAsync and Set don't yield.
    transactionSaveData.Set("someOtherKey", "someOtherValue");

    Console.WriteLine(saveData.Get<string>("someKey")) // someValue - The transaction has not completed.
    Console.WriteLine(transactionSaveData.Get<string>("someKey")) // someNewValue - Visible within the transaction.
});
Console.WriteLine(saveData.Get<string>("someKey")) // someNewValue
```

For both, `OpenCloudResponseException` will be thrown for issues.
`OpenCloudUnauthorizedException` will be thrown if the API key is
suddenly invalid after working before while `OpenCloudInsufficientScopeException`
will be thrown for a missing permission. `OpenCloudResponseException`
without a subclass in most cases will be unexpected.

## Contributing
Both issues and pull requests are accepted for this project.

## License
Nexus Data Store .NET is available under the terms of the MIT 
Licence. See [LICENSE](LICENSE) for details.