FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ["qluLib.net.csproj", "./"]
RUN dotnet restore "qluLib.net.csproj"

COPY . .

RUN dotnet publish "qluLib.net.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "qluLib.net.dll"]