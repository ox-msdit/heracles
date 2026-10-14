# Heracles

`heracles` is a command-line tool for querying, adding, modifying and deleting records in the University of Oxford Hydra DNS. It is primarily used to automate DNS-01 certificate validation with Certbot, allowing certificates to be provisioned on hosts that cannot expose port 80 to the internet.

This fork focuses on Linux development and deployment. It provides a fully static Linux x86-64 executable built using .NET 10 Native AOT, tested on Ubuntu 18.04 through 26.04. The original cross-platform project, including historical Windows and macOS builds, is available at https://github.com/WIMM-IT/heracles.

## Getting Heracles

The latest Linux executable is available in Releases 👉 or from `Heracles.Console/Binaries/Linux-x64/heracles`. Alternatively, you can compile your own executable using Docker, as described below. No .NET runtime or additional libraries are required on the target system, although HTTPS requires a functioning CA certificate trust store.

## Requirements

You must create a Hydra API token, as described at https://wiki.it.ox.ac.uk/networks/HydraTokens. Heracles also requires the following environment variables:

- `HYDRA_URI`: URI for the production or sandpit Hydra API, as documented at https://wiki.it.ox.ac.uk/networks/HydraAPI. Must end with `/ipam/`, including the trailing slash.
- `HYDRA_TOKEN`: Authentication token in the format `unit/user:encodedpassword`, including the colon.

### Safety

By default, Heracles refuses to update or delete more than one DNS record in a single operation. To override this protection, set the environment variable `HYDRA_UNSAFE` to any non-null value. Other bulk operations are supported without this restriction.

## Automated certificate requests

Heracles can automate certificate requests using Certbot and DNS-01 authentication. The Certbot integration scripts are provided in the `Scripts` directory. See the comments at the beginning of each script for configuration and usage instructions.

## Building

### Compiling a static Linux executable

A fully static Linux executable can be compiled using Docker, without installing the .NET SDK or compiler tools locally. The resulting binary has no glibc dependency and has been tested against Ubuntu 18.04 through 26.04.

1. Install Docker with the Compose plugin.
2. Run:

```bash
docker compose build && docker compose run --rm export
```

The executable will be written to:

```text
Heracles.Console/Binaries/Linux-x64/heracles
```

The Dockerfile uses .NET 10 Native AOT with musl, statically linked OpenSSL and invariant globalization to produce a statically linked x86-64 executable.

### Compiling directly

Heracles can also be compiled directly on a Linux development host with the .NET 10 SDK and the Native AOT compiler prerequisites installed. See the [Microsoft Native AOT documentation](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/#prerequisites) for the required compiler tools.

Run:

```bash
dotnet publish -c Release
```

This produces a native executable for the host operating system and architecture. For the portable, fully static Linux build, use the Docker procedure above.

## Testing

### Integration testing

Integration tests run directly on the development host and require the .NET 10 SDK and access to the Hydra API. Credentials are supplied through the local `env` file. Run the following command from the repository root:

```bash
(set -a; source ./env; set +a; dotnet test)
```

The subshell exports the environment variables to the test process without modifying the current shell environment. The tests interact with the live Hydra API and may intermittently fail due to connection refusals currently under investigation.

### Linux compatibility testing

The static executable can be tested against Ubuntu 18.04, 20.04, 22.04, 24.04 and 26.04 using Docker Compose. After building and exporting the executable, run:

```bash
for version in 1804 2004 2204 2404 2604; do
    echo "Testing Ubuntu ${version}"
    docker compose run --rm --no-deps "test-${version}" || exit 1
done
```

Each test performs an authenticated HTTPS query against the Hydra API using credentials from the local `env` file. The Ubuntu test containers install CA certificates before running Heracles. No .NET runtime is required.

## Examples

### Example 1 - Searching for records matching a string

Returns a JSON list of matching records, if any. Searches match any keyless data field and include partial matches. See https://www.networks.it.ox.ac.uk/university/ipam/help/searching for details. Example search terms include `192.168.10.`, `_acme-challenge`, `web.foo.ox.ac.uk` and `CNAME`.

```bash
$ heracles search _acme
[
  {
    "big_endian_labels": [
      "uk",
      "ac",
      "ox",
      "unit",
      "web",
      "_acme-challenge"
    ],
    ...
  }
]
```

### Example 2 - Getting matching records

Takes a JSON list containing one or more entries to retrieve, either on standard input or as the second argument. The `id`, `content`, `hostname` and `type` fields are required. Returns a JSON list of the retrieved entries. Primarily useful for safely testing command pipelines.

```bash
$ heracles search _acme-challenge.web | heracles get
[
  {
    "big_endian_labels": [
      "uk",
      "ac",
      "ox",
      "unit",
      "web",
      "_acme-challenge"
    ],
    ...
  }
]
```

### Example 3 - Adding new records

Takes a JSON list containing one or more entries to add, either on standard input or as the second argument. The `content`, `hostname` and `type` fields are required. Returns a JSON list of the affected entries.

```bash
$ echo '[{ "comment": "Test", "content": "foofoofoofoofoofoofoofoo", "hostname": "_acme-challenge.foo.unit.ox.ac.uk.", "type": "TXT" }]' | heracles add
[
  {
    "big_endian_labels": [
      "uk",
      "ac",
      "ox",
      "unit",
      "foo",
      "_acme-challenge"
    ],
    "bind_rdata": "\u0022foofoofoofoofoofoofoofoo\u0022",
    "comment": "Test",
    "content": "foofoofoofoofoofoofoofoo",
    ...
  }
]
```

### Example 4 - Updating a record

Takes a JSON list containing one or more entries to modify, either on standard input or as the second argument. The `id`, `content`, `hostname`, `type` and any properties to modify are required. Updating more than one record requires the `HYDRA_UNSAFE` environment variable. Returns a JSON list of the affected entries.

```bash
$ heracles search _acme-challenge.foo | sed 's/comment": "Test"/comment": "Another Test"/' | heracles update
[
  {
    "big_endian_labels": [
      "uk",
      "ac",
      "ox",
      "unit",
      "foo",
      "_acme-challenge"
    ],
    "bind_rdata": "\u0022foofoofoofoofoofoofoofoo\u0022",
    "comment": "Another Test",
    "content": "foofoofoofoofoofoofoofoo",
    ...
  }
]
```

### Example 5 - Deleting a record

Takes a JSON list containing one or more entries to delete, either on standard input or as the second argument. The `id`, `content`, `hostname` and `type` fields are required. Deleting more than one record requires the `HYDRA_UNSAFE` environment variable. Returns a JSON list of the affected entries.

```bash
$ heracles search _acme-challenge.foo | heracles delete
[
  {
    "big_endian_labels": [
      "uk",
      "ac",
      "ox",
      "unit",
      "foo",
      "_acme-challenge"
    ],
    "bind_rdata": "\u0022foofoofoofoofoofoofoofoo\u0022",
    "comment": "Another Test",
    "content": "foofoofoofoofoofoofoofoo",
    ...
  }
]

$ heracles search _acme-challenge.foo
[]
```