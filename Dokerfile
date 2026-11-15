FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DessertOrders/DessertOrders.csproj", "DessertOrders/"]
RUN dotnet restore "DessertOrders/DessertOrders.csproj"
COPY . .
WORKDIR "/src/DessertOrders"
RUN dotnet build "DessertOrders.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DessertOrders.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DessertOrders.dll"]