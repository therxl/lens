# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file
# Use simple COPY form which is more robust in some CI/buildkit environments
## Copy source code (copy all to avoid context mismatch in some CI environments)
## Copy source code from the current build context (./backend/LensApi)
COPY . ./
 
# Diagnostics: list files, sizes, and a byte-dump fallback to identify corruption/encoding
RUN echo "=== PWD ===" && pwd || true
RUN echo "=== LS -la ===" && ls -la || true
RUN echo "=== FILE SIZE (bytes) ===" && wc -c LensApi.csproj || true
RUN echo "=== HEX DUMP (first 512 bytes) ===" && (command -v xxd >/dev/null 2>&1 && xxd -l 512 LensApi.csproj || command -v hexdump >/dev/null 2>&1 && hexdump -n 512 -C LensApi.csproj || od -An -t x1 -N512 LensApi.csproj) || true

# Diagnostic: print the project file to the build logs to debug CI context issues
RUN echo "---- Begin LensApi.csproj ----" && sed -n '1,200p' LensApi.csproj || true && echo "---- End LensApi.csproj ----"

RUN dotnet restore "LensApi.csproj"
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
