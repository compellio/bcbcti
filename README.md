# BCBCTI Server

> Note: this is an experimental project

## Decisions

### Iteration #1

#### Decisions

- STIX Object modifications are rejected until the previous version gets confirmed by the Registry API. Queues solve this issue but are not implemented in this version.

#### Risks

- DateTime string serialization. TAXII and STIX have different requirements. For simplicity, this project uses microsecond precision for all recorded timestamps.
  - TAXII 2.1 requires RFC 3339-formatted timestamps, with **microsecond** precision, in UTC (with the Z designation): `YYYY-MM-DDTHH:MM:SS.ssssssZ`. See https://docs.oasis-open.org/cti/taxii/v2.1/os/taxii-v2.1-os.html#_Toc26285787
  - STIX 2.1 requires RFC 3339-formatted timestamps, with **optional** sub-second precision, in UTC (with the Z designation). See https://docs.oasis-open.org/cti/stix/v2.1/cs02/stix-v2.1-cs02.html#_ksbm2nost85y
