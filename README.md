[![NuGet version (SoftCircuits.Silk)](https://img.shields.io/nuget/v/MorseSharp?color=green)](https://www.nuget.org/packages/MorseSharp/)
# MorseSharp
MorseSharp is a simple lightweight .NET library to convert **english** and **kurdish** sentences to morse code.

## Installation
Use nuget package manager to install [MorseSharp](https://www.nuget.org/packages/MorseSharp) using CLI.
```bash
Install-Package MorseSharp
```
## Usage
1.You can use MorseSharp easily just by instantiating `MorseTextConverter` class

```C#
using MorseSharp;
MorseTextConverter converter = new MorseTextConverter();
```
2.You can convert any sentence by just calling asynchronous method `ConvertToMorse`

```C#
using MorseSharp;
var morse = await converter.ConvertToMorse;
```
## Example 
This piece of code convert a simple text to morse code and then show it to the console:
```C#
using System;
using MorseSharp;

MorseTextConverter converter = new();
var morse = await converter.ConvertToMorse();

if(morse is not null){
   Console.WriteLine(morse);
}
