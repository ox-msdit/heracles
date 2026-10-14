# Contributing to Heracles

Contributions to `heracles` are welcome. This fork focuses on maintaining the Linux version, particularly its use with Certbot for automated DNS-01 certificate validation.

## Testing

Heracles is an established application used in production. The Linux executable is built using .NET 10 Native AOT and has been tested on Ubuntu 18.04 through 26.04.

Contributions that improve reliability, compatibility or test coverage are welcome. See the README for instructions on running the integration and Linux compatibility tests.

## Suggestions for improvement

Heracles is largely feature-complete, but suggestions for improvements, particularly those addressing reliability or maintainability, are welcome.

## Improving the certificate request scripts

The Certbot scripts are used in production and can be difficult to test safely. Contributions that fix bugs, improve error handling or increase reliability are particularly welcome.

## Writing unit tests

Additional test coverage is welcome, particularly for error handling and edge cases. Refactoring `Heracles.Lib` or `Heracles.Console` to improve testability may also be useful, provided existing functionality is preserved.