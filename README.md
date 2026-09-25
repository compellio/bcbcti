# BCBCTI Server

[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](LICENSE)
[![STIX/TAXII](https://img.shields.io/badge/STIX%2FTAXII-2.1-orange)](https://oasis-open.github.io/cti-documentation/)

BCBCTI (Blockchain-Based Cyber Threat Intelligence) is a cyber threat intelligence (CTI) observability tool that integrates with [STIX/TAXII 2.1](https://oasis-open.github.io/cti-documentation/) threat sources to provide an immutable and verifiable audit trail of detected indicators.

The BCBCTI server exposes a [TAXII 2.1](https://docs.oasis-open.org/cti/taxii/v2.1/os/taxii-v2.1-os.html) collection interface and collects threat intelligence indicators in [STIX 2.1](https://docs.oasis-open.org/cti/stix/v2.1/cs02/stix-v2.1-cs02.html) format. Each object it receives is registered as a Tokenized Asset Record (TAR) through [Compellio](https://compellio.com)'s Gateway Registry API and anchored on a public or private blockchain network, without publishing the indicator itself.

**Status:** experimental. BCBCTI is a reference implementation of a Registry API integration, not a production-ready product.

## Decisions

### Iteration #1

#### Decisions

- STIX Object modifications are rejected until the previous version gets confirmed by the Registry API. Queues solve this issue but are not implemented in this version.

## Licence

Licensed under the [Apache License 2.0](LICENSE).
