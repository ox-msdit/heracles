# Build static Native AOT executable
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine

RUN apk add --no-cache \
    clang \
    build-base \
    zlib-dev \
    zlib-static \
    binutils \
    file \
    cmake \
    openssl-dev \
    openssl-libs-static

WORKDIR /heracles

COPY heracles.sln ./
COPY Heracles.Console/ ./Heracles.Console/
COPY Heracles.Lib/ ./Heracles.Lib/
COPY Heracles.UnitTests/ ./Heracles.UnitTests/

RUN dotnet publish Heracles.Console/Heracles.Console.csproj \
    -c Release \
    -r linux-musl-x64 \
    -p:PublishAot=true \
    -p:StaticExecutable=true \
    -p:StaticExecutableWithDynamicLibC=false \
    -p:InvariantGlobalization=true \
    -p:StaticOpenSslLinking=true \
    -o /publish

# Verify static linking
RUN ! readelf -l /publish/heracles | grep -q INTERP && \
    ! readelf -d /publish/heracles | grep -q NEEDED
