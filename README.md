# Tiger.Types.JSON

## What It Is

Tiger.Types.JSON is a library that allows some of the Tiger types to be mapped to or from JSON in useful ways.

## Why You Want It

Tiger types are unfamiliar to the JSON.NET serialization library, and due to the way that they represent values, require a little help to serialize or deserialize properly. This is similar to serializing a type of `Nullable<T>` (also written as `T?`). One could imagine an implementation of `Option<int>` called `Count` that serializes as a property of another object like this:

```json
{
  "count": {
    "value": 42,
    "hasValue": true
  }
}
```

…which would require a client to create a very strange DTO or take a dependency on the Tiger types. (For various, difficult-to-believe reasons, the latter may be undesirable.)

Because all types in JSON are effectively nullable or optional, a superior serialization is this:

```json
{
  "count": 42
}
```

…where the None state is represented either by the JSON value `null` or by omitting the key entirely. This allows a client to implement a DTO with a property of type `int?` which can have a value or be `null`, of type `int` which will receive a default value (in this case, `0`), or of type `Option<int>`.

The supported operations for types are currently these:

- `Option`: Serialize, deserialize
- `Either`: Serialize

## How You Develop It

This project is using the standard [`dotnet`](https://dot.net) build tool. A brief primer:

- Restore NuGet dependencies: `dotnet restore`
- Build the entire solution: `dotnet build`
- Run all unit tests: `dotnet test`
- Pack for publishing: `dotnet pack -o "$(pwd)/dist"`

The parameter `--configuration` (shortname `-c`) can be supplied to the `build`, `test`, and `pack` steps with the following meaningful values:

- “Debug” (the default)
- “Release”

This repository is attempting to use the [GitFlow](http://jeffkreeftmeijer.com/2010/why-arent-you-using-git-flow/) branching methodology. Results may be mixed, please be aware.

## Thank You

Seriously, though. Thank you for using this software. The author hopes it performs admirably for you.
