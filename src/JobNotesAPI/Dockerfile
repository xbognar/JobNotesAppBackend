# Use the official .NET 8.0 SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj files and restore as distinct layers
COPY ["src/JobNotesAPI/JobNotesAPI.csproj", "src/JobNotesAPI/"]
COPY ["src/DataAccess/DataAccess.csproj", "src/DataAccess/"]
RUN dotnet restore "src/JobNotesAPI/JobNotesAPI.csproj"

# Copy the rest of the source code and build the app
COPY . .
WORKDIR "/app/src/JobNotesAPI"
RUN dotnet build "JobNotesAPI.csproj" -c Release -o /app/build

# Publish the application to the /app/publish folder
RUN dotnet publish "JobNotesAPI.csproj" -c Release -o /app/publish

# Use the official .NET 8.0 runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "JobNotesAPI.dll"]
