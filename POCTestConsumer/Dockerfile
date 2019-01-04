FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY POCTestConsumer/POCTestConsumer.csproj POCTestConsumer/
COPY POCInterfaces/POCInterfaces.csproj POCInterfaces/
RUN dotnet restore POCTestConsumer/POCTestConsumer.csproj
COPY . .
WORKDIR /src/POCTestConsumer
RUN dotnet build POCTestConsumer.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish POCTestConsumer.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "POCTestConsumer.dll"]
