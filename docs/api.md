# API

The BCBCTI server implements a subset of the [TAXII 2.1](https://docs.oasis-open.org/cti/taxii/v2.1/os/taxii-v2.1-os.html) API specification.

> [!CAUTION]
> BCBCTI is meant to be deployed as an internal service and **does not implement
authentication**. Anything that can reach a collection endpoint can write to it. If you plan to deploy BCBCTI publicly, you should place it behind your own authentication layer (e.g. reverse proxy, API gateway, etc.).

It serves a single API root at `/api` and a single `default` collection. The following endpoints are implemented:

- [x] `GET /taxii2`: server discovery
- [x] `GET /api`: get API root information
- [x] `GET /api/status/{status-id}`: get status (can be used to monitor submissions)
- [x] `GET /api/collections`: list collections
- [x] `GET /api/collections/{id}`: get a collection
- [x] `GET /api/collections/{id}/manifest`: list STIX object manifests
- [x] `GET /api/collections/{id}/objects`: list STIX objects
- [x] `POST /api/collections/{id}/objects`: submit STIX objects
- [ ] `GET /api/collections/{id}/objects/{object-id}`
- [ ] `DELETE /api/collections/{id}/objects/{object-id}`
- [ ] `GET /api/collections/{id}/objects/{object-id}/versions`

Pagination with `?added_after` and `?limit` query params, and the `X-TAXII-Date-Added-First` and `X-TAXII-Date-Added-Last` HTTP headers is supported. 
Filtering with `?match[]=` query params is not supported, and the server will return an error HTTP status code.

BCBCTI also returns custom properties linking TAXII resources with their associated Registry API registrations. Those properties are prefixed with `x_` as per [section 7.1](https://docs.oasis-open.org/cti/taxii/v2.1/os/taxii-v2.1-os.html#_Toc31107547) of the TAXII 2.1 specification:

| Property                       | Returned on      | Description                                                    |
|--------------------------------|------------------|----------------------------------------------------------------|
| `x_bcbcti_registry_receipt_id` | Status, Manifest | The receipt assigned by the Registry API for the registration. |
| `x_bcbcti_registry_tar_id`     | Manifest         | The TAR Id issued by the Registry API.                         |
| `x_bcbcti_registry_version_id` | Manifest         | The TAR version corresponding to the entry.                    |

You may also use those values to call the Registry API directly. Read the [Registry API documentation](https://docs.compellio.com/registry/) for more details.

> [!TIP]
> You can find a set of example requests in [BCBCTI.http](./../BCBCTI.http)

## Limitations

- STIX Object modifications will be rejected until the previous submitted version's registration gets confirmed by the Registry API. Those objects will fail with a `pending registration` error code.
