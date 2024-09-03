# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy the solution file and all project files
COPY librarysystem.sln ./
COPY api/api.csproj ./api/
COPY service/service.csproj ./service/
COPY infrastructure/infrastructure.csproj ./infrastructure/

# Restore dependencies for all projects
RUN dotnet restore

# Copy the remaining files into the container
COPY . ./

# Build the project
RUN dotnet publish api -c Release -o out

# Use the official ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Install PostgreSQL client tools
RUN apt-get update && apt-get install -y postgresql-client

# Copy the build output from the previous stage
COPY --from=build-env /app/out .

# Copy the entry script for database initialization
COPY entrypoint.sh /app/entrypoint.sh

# Make the entry script executable
RUN chmod +x /app/entrypoint.sh

# Expose port 80
EXPOSE 80

# Set environment variables for PostgreSQL
ENV DB_USER=postgres
ENV DB_PASSWORD=Password
ENV DB_NAME=todo_db
ENV DB_HOST=postgres
ENV pgconn=postgresql://${DB_USER}:${DB_PASSWORD}@${DB_HOST}/${DB_NAME}

# Set the entry point to use the initialization script and run the app
ENTRYPOINT ["/app/entrypoint.sh", "dotnet", "api.dll"]
