# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file
COPY ["LensApi.csproj", "./"]
RUN dotnet restore "LensApi.csproj"

# Copy source code
COPY . .
RUN dotnet build "LensApi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "LensApi.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
RUN apt-get update \
	&& apt-get install -y --no-install-recommends curl \
	&& rm -rf /var/lib/apt/lists/*
COPY --from=publish /app/publish .

EXPOSE 5126
ENTRYPOINT ["dotnet", "LensApi.dll"]
