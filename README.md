# Site Checker

## Features
Checks sites health

## Install
1. In root directory
```bash
dotnet restore
dotnet build
dotnet publish
```

2. Copy sites.db and users.db to 
```
src\SiteChecker\bin\Debug\netcoreapp2.0\publish\
```

3. Copy SQLite.Interop.dll from *src\SiteChecker\bin\Debug\netcoreapp2.0\x64* or *src\SiteChecker\bin\Debug\netcoreapp2.0\x86* to publish directory

## Usage

In src\SiteChecker\bin\Debug\netcoreapp2.0\publish\ 
```bash
dotnet SiteChecker.dll
```

## Additional
1. Default user
```
admin@checker.ru Qwe123!!
```
2. You can init your own databases with *.sql\init* scripts