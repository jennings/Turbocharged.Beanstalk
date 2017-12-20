.PHONY: all restore build test pack

all: build test

restore:
	dotnet restore

build: restore
	dotnet build

test:
	dotnet test src/Turbocharged.Beanstalk.Tests/Turbocharged.Beanstalk.Tests.csproj

pack:
	dotnet pack src/Turbocharged.Beanstalk/Turbocharged.Beanstalk.csproj --configuration Release
