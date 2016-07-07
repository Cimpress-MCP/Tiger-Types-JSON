# Tiger.Types

## What It Is

Tiger.JsonTypes is a library that extends `Tiger.Types` for JSON support.

## Why You Want It

The Newtonsoft.Json library has built-in support for may types, but not for `Option<TSome>`.  This library instructs Newtonsoft.Json how to treat `Option<TSome>` identically to `TSome?` (or `Nullable<TSome>`) in the case that `TSome: struct`, and identically to `TSome` in the case that `TSome: class`.  This allows a JSON document produced by an Option-aware library to be consumed by a non–Option-aware library, and <i lang="la">vice versa</i>.

## How You Develop It

You’re in the right place for that.  Once you have this directory forked and cloned, the provided Visual Studio solution file should contain everything you need to get going.  The NuGet packages will be restored and the NUnit unit tests will be detected (if your version of Visual Studio supports NUnit3, that is).  If you’re interested in the command-line builds, they use a system called [Cake](http://cakebuild.net).  While it is possible to run the cakefile (`build.cake`) directly, the preferred method is to run the build bootstrapper (`build.ps1`).  The build bootstrapper ensures that you have the development and testing tools installed in your environment.  It is a powershell script, so the way to execute it will vary by your command line.

- Powershell: `./build.ps1`
- cmd: `powershell ./build.ps1`
- bash: `powershell ./build.ps1`

This repository is attempting to use the [GitFlow](http://jeffkreeftmeijer.com/2010/why-arent-you-using-git-flow/) branching methodology.  Results may be mixed, please be aware.

## Thank You

Seriously, though.  Thank you for using this software.  The author hopes it performs admirably for you.
