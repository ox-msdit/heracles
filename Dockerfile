# Build static Native AOT executable
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

RUN apk add --no-cache \
    clang \
    build-base \
    zlib-dev \
    zlib-static \
    binutils \
    file

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
    -o /publish

# Verify static linking
RUN ! readelf -l /publish/heracles | grep -q INTERP && \
    ! readelf -d /publish/heracles | grep -q NEEDED

# Test Ubuntu compatibility
FROM ubuntu:18.04 AS test-1804
COPY --from=build /publish/heracles /heracles
RUN /heracles 2>&1 | grep -Fq 'USAGE: heracles search string'

FROM ubuntu:20.04 AS test-2004
COPY --from=build /publish/heracles /heracles
RUN /heracles 2>&1 | grep -Fq 'USAGE: heracles search string'

FROM ubuntu:22.04 AS test-2204
COPY --from=build /publish/heracles /heracles
RUN /heracles 2>&1 | grep -Fq 'USAGE: heracles search string'

FROM ubuntu:24.04 AS test-2404
COPY --from=build /publish/heracles /heracles
RUN /heracles 2>&1 | grep -Fq 'USAGE: heracles search string'

FROM ubuntu:26.04 AS test-2604
COPY --from=build /publish/heracles /heracles
RUN /heracles 2>&1 | grep -Fq 'USAGE: heracles search string'

# Final image, dependent on all compatibility tests
FROM build

COPY --from=test-1804 /heracles /tmp/test-1804
COPY --from=test-2004 /heracles /tmp/test-2004
COPY --from=test-2204 /heracles /tmp/test-2204
COPY --from=test-2404 /heracles /tmp/test-2404
COPY --from=test-2604 /heracles /tmp/test-2604