# Use the official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["src/JobNotesAPI/JobNotesAPI.csproj", "src/JobNotesAPI/"]
COPY ["src/DataAccess/DataAccess.csproj", "src/DataAccess/"]
RUN dotnet restore "src/JobNotesAPI/JobNotesAPI.csproj"

# Copy the rest of the source code and build the app
COPY . .
WORKDIR "/src/JobNotesAPI"
RUN dotnet build "JobNotesAPI.csproj" -c Release -o /app/build

# Publish the application to the /app/publish folder
FROM build AS publish
RUN dotnet publish "JobNotesAPI.csproj" -c Release -o /app/publish

# Use the official .NET runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobNotesAPI.dll"]
