# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file
COPY ["RecommendationsApi.csproj", "./"]
RUN dotnet restore "RecommendationsApi.csproj"

# Copy source code
COPY . .
RUN dotnet build "RecommendationsApi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "RecommendationsApi.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
RUN apt-get update \
	&& apt-get install -y --no-install-recommends curl \
	&& rm -rf /var/lib/apt/lists/*
COPY --from=publish /app/publish .

EXPOSE 5127
ENTRYPOINT ["dotnet", "RecommendationsApi.dll"]
