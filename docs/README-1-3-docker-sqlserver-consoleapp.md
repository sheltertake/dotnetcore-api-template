# Docker - SqlServer - Console application

 - In this branch I want to create a console application. 
 - It will connect with the database and query friends table. 
 - It will print foreach friend Hello {name} 

## Creation

```cmd

C:\VSTS\PLAYGROUND\testing-playground>mkdir src

C:\VSTS\PLAYGROUND\testing-playground>cd src

C:\VSTS\PLAYGROUND\testing-playground\src>mkdir 0-consoleapp-use-dockersql

C:\VSTS\PLAYGROUND\testing-playground\src>cd 0-consoleapp-use-dockersql


C:\VSTS\PLAYGROUND\testing-playground\src\0-consoleapp-use-dockersql>dotnet new console -n ConsoleAppDockerSql
The template "Console Application" was created successfully.

Processing post-creation actions...
Running 'dotnet restore' on ConsoleAppDockerSql\ConsoleAppDockerSql.csproj...
  Determining projects to restore...
  Restored C:\VSTS\PLAYGROUND\testing-playground\src\0-consoleapp-use-dockersql\ConsoleAppDockerSql\ConsoleAppDockerSql.csproj (in 179 ms).

Restore succeeded.
```

## EntityFrameworkCore help

 - (First EF Core Console Application)[https://www.entityframeworktutorial.net/efcore/entity-framework-core-console-application.aspx]


## Entity Framework Core + Code

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp3.1</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="3.1.9" />
  </ItemGroup>

</Project>
```

```csharp
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ConsoleAppDockerSql
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new FriendContext())
            {

               var friends = context.Friends.ToList();
                foreach(var friend in friends)
                {
                    Console.WriteLine($"Hello {friend.Name}");
                }
            }
        }
    }
    class Friend
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    class FriendContext : DbContext
    {
        public DbSet<Friend> Friends { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=127.0.0.1;Initial Catalog=Friends;Persist Security Info=True;User ID=sa;Password=yourStrong1234!Password;MultipleActiveResultSets=True;Application Name=ConsoleAppDockerSql");
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }
}

```

```cmd
C:\VSTS\PLAYGROUND\testing-playground\src\0-consoleapp-use-dockersql\ConsoleAppDockerSql>dotnet run
Hello Silvio
Hello Marco
Hello Luca

C:\VSTS\PLAYGROUND\testing-playground\src\0-consoleapp-use-dockersql\ConsoleAppDockerSql>dotnet publish -c Release
Microsoft (R) Build Engine version 16.7.0+7fb82e5b2 for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  All projects are up-to-date for restore.
  ConsoleAppDockerSql -> C:\VSTS\PLAYGROUND\testing-playground\src\0-consoleapp-use-dockersql\ConsoleAppDockerSql\bin\Release\netcoreapp3.1\ConsoleAppDockerSql.dll
  ConsoleAppDockerSql -> C:\VSTS\PLAYGROUND\testing-playground\src\0-consoleapp-use-dockersql\ConsoleAppDockerSql\bin\Release\netcoreapp3.1\publish\

C:\VSTS\PLAYGROUND\testing-playground\src\0-consoleapp-use-dockersql\ConsoleAppDockerSql>dotnet bin\Release\netcoreapp3.1\publish\ConsoleAppDockerSql.dll
Hello Silvio
Hello Marco
Hello Luca

C:\VSTS\PLAYGROUND\testing-playground\src\0-consoleapp-use-dockersql\ConsoleAppDockerSql>

```