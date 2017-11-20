# Dapper.Stream
Dapper extension that enables you to put results of T-SQL query into a stream.
This functionality is important when you need to create REST API directly from the results of T-SQL queries.

## Installation

[Dapper.Stream](https://www.nuget.org/packages/Dapper.Stream) is available as NuGet package. You can install it using NuGet **Install-Package** command:

```
Install-Package Dapper.Stream
```

Or you can use .Net CLI:
```
dotnet add package Dapper.Stream
```

Once you install required packages, you need to add **using** directive in your C# file that references *Dapper* namesapce:
```
using Dapper;
```

# Usage

Let's assume that you have a table in SQL Server with the following schema:

```
CREATE TABLE Product (
	ProductID int IDENTITY PRIMARY KEY,
	Name nvarchar(50) NOT NULL,
	Price money NOT NULL,
	Quantity int NULL,
	Color nvarchar(30)
)
```

[Dapper.Stream](https://www.nuget.org/packages/Dapper.Stream) enables you to execute a T-SQL query against this table and put results into an Output Stream.

## Exporting content of the table to XML file

The goal is to select all rows from the table and store them as XML content in a file. SQL Server has FOR XML clause that can format query results as XML, and [Dapper.Stream](https://www.nuget.org/packages/Dapper.Stream) enables you to store results of the query into a file stream:
```
using (FileStream fs = File.Create(path))
{
    var QUERY = 
@"SELECT ProductID, Name, Price, Quantity, Color
FROM Product
FOR JSON PATH";

    connection.QueryInto(fs, QUERY);
}
```

## Creating JSON REST API

The goal is to create a REST API in ASP.NET MVC Controller that returns list of products from the table formatted as JSON. SQL Server 2016+ has FOR JSON clause that formats query results as JSON text, and [Dapper.Stream](https://www.nuget.org/packages/Dapper.Stream) enables you to put results of the query into an HTTP response of the REST API:
```
[HttpGet]
public void Get()
{
    var QUERY = 
@"SELECT ProductID, Name, Price, Quantity, Color
FROM Product
FOR JSON PATH";

    connection.QueryInto(Response.Body, QUERY);
}
```

# Reference

[Dapper.Stream](https://www.nuget.org/packages/Dapper.Stream) is used in [SQL Server GitHub sample project](https://github.com/Microsoft/sql-server-samples/tree/master/samples/features/json/Dapper-Orm).