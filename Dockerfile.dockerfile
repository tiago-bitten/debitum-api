FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /src

COPY . . 

RUN dotnet restore "Debitum.sln"

RUN dotnet publish "src/Adapters/Driving/API/API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app
COPY --from=build-env /app/publish .

ENTRYPOINT ["dotnet", "API.dll"]