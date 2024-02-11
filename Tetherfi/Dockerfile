# Use the .NET 6 SDK image as a base
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy the solution file and restore dependencies
COPY *.sln ./
# Copy the project files
COPY Tetherfi/Tetherfi.csproj /app/Tetherfi/
COPY Tetherfi.Services/Tetherfi.Services.csproj /app/Tetherfi.Services/
COPY Tetherfi.Data/Tetherfi.Data.csproj /app/Tetherfi.Data/
COPY Tetherfi.Common/Tetherfi.Common.csproj /app/Tetherfi.Common/
COPY Tetherfi.Helpers/Tetherfi.Helpers.csproj /app/Tetherfi.Helpers/
COPY Tetherfi.Test/Tetherfi.Test.csproj /app/Tetherfi.Test/
RUN dotnet restore

# Copy the project files and build the application
COPY . ./
RUN dotnet publish -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "Tetherfi.dll"]
