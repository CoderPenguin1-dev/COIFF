# Cinder Engine
The Cinder Engine is an engine purpose-built for choice-based storygames made in .NET 10.0.

Made to be easy yet flexible, it allows the writer to just worry about writing without needing to worry about actual programming unless they wish for extended logic.

The engine itself does implement Lua as a scripting language. Information can be found in the [Documentation](../Docs/README.MD).

Cinder Engine requires a frontend for any story file to be viewed. The official one is [Flame](../Flame).

## Implementation
For any frontend implementation, you need to implement at bare minimum:
* A way to show the text.
  * `GetNodeText()`
* A way to show and interact with options
  * `GetNodeOptions()` for getting the option labels.
  * `ParseSelectedOption(int)` for interaction.

You can see Flame's implementation for reference on how to achieve this.

To actually implement the namespace into your code, you could either of the following:
* Build the class library DLL.
* Reference the project itself.

For simplicity, I suggest doing the latter.
Currently there is no NuGet package.

## Building
### Prerequisites
* [.NET 10.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
* [Lua-CSharp](https://github.com/nuskey8/Lua-CSharp)

Clone the repo:

    git clone https://github.com/CoderPenguin1-dev/COIFF.git

Move into the build directory:

    cd COIFF/CinderEngine

Build the library in Debug:

    dotnet build

The resulting binary can be found in `CinderEngine\bin\net10.0`.