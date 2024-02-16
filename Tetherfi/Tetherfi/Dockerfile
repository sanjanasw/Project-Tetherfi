#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tetherfi/Tetherfi.csproj", "Tetherfi/"]
COPY ["Tetherfi.Services/Tetherfi.Services.csproj", "Tetherfi.Services/"]
COPY ["Tetherfi.Common/Tetherfi.Common.csproj", "Tetherfi.Common/"]
COPY ["Tetherfi.Data/Tetherfi.Data.csproj", "Tetherfi.Data/"]
COPY ["Tetherfi.Helpers/Tetherfi.Helpers.csproj", "Tetherfi.Helpers/"]
RUN dotnet restore "Tetherfi/Tetherfi.csproj"
COPY . .
WORKDIR "/src/Tetherfi"
RUN dotnet build "Tetherfi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tetherfi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tetherfi.dll"]
