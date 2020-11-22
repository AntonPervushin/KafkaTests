#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 44305

ENV ASPNETCORE_URLS=http://+:44305

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["KafkaTests.Producer/KafkaTests.Producer.csproj", "KafkaTests.Producer/"]
COPY ["KafkaTests.Abstractions/KafkaTests.Abstractions.csproj", "KafkaTests.Abstractions/"]
COPY ["KafkaTests.Implementations/KafkaTests.Implementations.csproj", "KafkaTests.Implementations/"]
RUN dotnet restore "KafkaTests.Producer/KafkaTests.Producer.csproj"
COPY . .
WORKDIR "/src/KafkaTests.Producer"
RUN dotnet build "KafkaTests.Producer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KafkaTests.Producer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KafkaTests.Producer.dll"]