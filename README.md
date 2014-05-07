net-tools
=========

Net Industry repository/playground for any .NET reusable code.


[![Build Status](https://travis-ci.org/zidad/net-tools.png?branch=master)](https://travis-ci.org/zidad/net-tools)

Net.Core 
-
Contains a set of .NET framework namespaces with no other dependencies, such as:

- DependencyInjection: generic attributes for dependency injection registration
- Collections: mostly LINQ extension methods to make working with LINQ to objects easier
- Enums: Easier Enum parsing/handling/attributing
- Reflection: Improve working with attributes
- Text: String formatting and checking utilities
- ToExtensions: object extensions to allow easier conversion from one object to another

[install from nuget:](http://nuget.org/packages/Net.Core)

    install-package Net.Core

Net.Autofac 
-
Contains modules and classes to simplify setting up Autofac. Automatic scanning and registration of types based on attributes. Helps preventing me from having to do registration in most common scenario's 

[install from nuget:](http://nuget.org/packages/Net.Autofac)

    install-package Net.Autofac

Net.EasyNetQ 
-
Contains experiments and additions for EasyNetQ (depends on Autofac too), to implement a Nancy-like pipeline for message processing in Autofac, and add (very basic) Correlation, Locking and Saga support

[install from nuget:](http://nuget.org/packages/Net.EasyNetQ)

    install-package Net.EasyNetQ
Net.FluentMigrator 
-
Contains helpers to simplify the usage of [FluentMigrator](https://github.com/schambers/fluentmigrator "FluentMigrator")

[install from nuget:](http://nuget.org/packages/Net.FluentMigrator)
    
	install-package Net.FluentMigrator

Net.Web 
-
Contains helpers for Asp.Net MVC

[install from nuget:](http://nuget.org/packages/Net.Web)

    install-package Net.FluentMigrator
