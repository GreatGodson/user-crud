# Use the .NET SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy and restore the project
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and publish
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Set the port Render should expose
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# 👇 ENTRYPOINT points to the compiled DLL
ENTRYPOINT ["dotnet", "MyFirstApi.dll"]