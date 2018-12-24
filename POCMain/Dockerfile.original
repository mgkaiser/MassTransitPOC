FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["POCMain/POCMain.csproj", "POCMain/"]
COPY ["POCInterfaces/POCInterfaces.csproj", "POCInterfaces/"]
RUN dotnet restore "POCMain/POCMain.csproj"
COPY . .
WORKDIR "/src/POCMain"
RUN dotnet build "POCMain.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "POCMain.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "POCMain.dll"]