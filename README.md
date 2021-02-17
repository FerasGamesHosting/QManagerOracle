# QManagerOracle

[![Build status](https://ci.appveyor.com/api/projects/status/1n6q8aql72cv8oom?svg=true)](https://ci.appveyor.com/project/rafaelandrade74/qmanageroracle)
[![Version](https://img.shields.io/nuget/v/QManagerOracle?color=gree)](https://www.nuget.org/packages/QManagerOracle/)
[![Issues](https://img.shields.io/github/issues/FerasGamesHosting/QManagerOracle)](https://github.com/FerasGamesHosting/QManagerOracle/issues)
[![Downloads](https://img.shields.io/nuget/dt/QManagerOracle?color=gree)](https://www.nuget.org/packages/QManagerOracle/)
[![Forks](https://img.shields.io/github/forks/FerasGamesHosting/QManagerOracle?color=gree)](https://github.com/FerasGamesHosting/QManagerOracle/network/members)
[![License](https://img.shields.io/github/license/FerasGamesHosting/QManagerOracle)](https://github.com/FerasGamesHosting/QManagerOracle/blob/master/LICENSE)




#How to use
```csharp
SQLPlus sql = new SQLPlus();

sql.ExecuteAsync(new ParamsScript()
{
	Parameters = "",
	ScriptDir = Environment.CurrentDirectory,
	ScriptName = "criar_tabela.sql"
},
new ParamsDB() 
{
	CriarNovaJanela = true,
	IPAdress = "192.168.0.4",
	PassDB = "1234"
});

```

