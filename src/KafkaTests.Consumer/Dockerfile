#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["KafkaTests.Consumer/KafkaTests.Consumer.csproj", "KafkaTests.Consumer/"]
COPY ["KafkaTests.Abstractions/KafkaTests.Abstractions.csproj", "KafkaTests.Abstractions/"]
COPY ["KafkaTests.Implementations/KafkaTests.Implementations.csproj", "KafkaTests.Implementations/"]
RUN dotnet restore "KafkaTests.Consumer/KafkaTests.Consumer.csproj"
COPY . .
WORKDIR "/src/KafkaTests.Consumer"
RUN dotnet build "KafkaTests.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KafkaTests.Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KafkaTests.Consumer.dll"]