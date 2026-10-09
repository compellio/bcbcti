# BCBCTI Server

[![License](https://img.shields.io/github/license/compellio/bcbcti)](LICENSE.txt)
[![Releases](https://img.shields.io/github/v/release/compellio/bcbcti?include_prereleases)](https://github.com/compellio/bcbcti/releases)
[![STIX/TAXII](https://img.shields.io/badge/STIX%2FTAXII-2.1-orange)](https://oasis-open.github.io/cti-documentation/)

BCBCTI (Blockchain-Based Cyber Threat Intelligence) is a cyber threat intelligence (CTI) observability tool that integrates with [STIX/TAXII 2.1](https://oasis-open.github.io/cti-documentation/) threat sources to provide an immutable and verifiable audit trail of detected indicators.

The BCBCTI server exposes a [TAXII 2.1](https://docs.oasis-open.org/cti/taxii/v2.1/os/taxii-v2.1-os.html) collection interface and collects threat intelligence indicators in [STIX 2.1](https://docs.oasis-open.org/cti/stix/v2.1/cs02/stix-v2.1-cs02.html) format. Each object it receives is registered as a Tokenized Asset Record (TAR) through the [Compellio](https://compellio.com) Registry API and anchored on a public or private blockchain network, without publishing the indicator itself.

> [!IMPORTANT]
> This is a pre-release version of the BCBCTI server which does not make calls the Registry API by default. It can be used to test the behaviour of the available TAXII endpoints.

## Architecture

[//]: # (TODO high level overview)

Read the [architecture documentation](docs/architecture.md) for more details.

## Getting Started

Your can pull the BCBCTI server's container image from the GitHub Container registry. 

```shell
docker pull ghcr.io/compellio/bcbcti:latest
```

We recommend using the Docker Compose example in [compose.yaml](compose.yaml) and edit it as required for your deployments.

The BCBCTI server depends on an instance of the Registry API to register TARs. You can either host an instance yourself, or use the Compellio Gateway closed alpha Registry API endpoint.

Read the [configuration documentation](docs/configuration.md) for more details.

## API

[//]: # (TODO quick overview of endpoints and x_ custom variables + link to docs listing API endpoint and/or openapi file)

Read the [API documentation](docs/api.md) for more details.

[//]: # (TODO add contributing section?)

## License

This project is licensed under the [Apache License 2.0](LICENSE.txt).
