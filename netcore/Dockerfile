FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN find .
RUN dotnet publish -c Release -o out
RUN find .

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
EXPOSE 5000
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "netcore.dll"]