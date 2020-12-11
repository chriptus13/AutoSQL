# AutoSQL

AutoSQL is a .NET library for creating SQL data mappers for any entity using Reflection or Emit APIs.

This project was developed in Ambientes Virtuais de Execução (Virtual Execution Enviromnents) subject in [ISEL](https://www.isel.pt) during my degree in Computer Science and Computer Engineering.

There were three phases in the project:

* [Phase 1](/AVE_1718v_LI42D_T1.pdf) => The objective was to implement the data mappers using .NET `System.Reflection` API
* [Phase 2](/AVE_1718v_LI42D_T2.pdf) => The objective was to implement the data mappers using .NET `System.Reflection.Emit` API
* [Phase 3](/AVE_1718v_LI42D_T3.pdf) => The objective was to change the `IDataMapper` interface to support generic types and lazy iterators

## Learning objectives

* Reflection
* Dynamic intermediate code analysis
* Dynamic code generation
* Delegates
* Generic types
* [`IEnumerator<T>`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1)'s
* [Yield](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/yield)
* Unit testing

## Technologies used

* C#
* .NET `System.Reflection` API
* .NET `System.Reflection.Emit` API
* Microsoft intermediate language (MSIL)
* SQL

## Authors

This project was developed with [Cláudio Bartolomeu](https://github.com/cbartolomeu) and [Samuel Sampaio](https://github.com/SamuelSampaio98).