# Configuration

[//]: # (TODO)

> [!CAUTION]
> BCBCTI is meant to be deployed as an internal service and **does not implement
authentication**. Anything that can reach a collection endpoint can write to it. If you plan to deploy BCBCTI publicly, you should place it behind your own authentication layer (e.g. reverse proxy, API gateway, etc.).

## Environment Variables

[//]: # (TODO common env variables, essentially everything under BCBCTI)

### Registry API

The BCBCTI server can work with either the open source [DCAP.Web](https://github.com/compellio/DCAP.Web/pkgs/container/dcap-web) server and the [Compellio Gateway](https://console.gateway.compellio.com/) Registry API service.

You can find more instructions on running the DCAP.Web service at <https://github.com/compellio/DCAP.Web>. In the context of the BCBCTI, you may skip the "Data initialization" section.

You can configure BCBCTI according to the option you choose using the following variables:

| Environment Variable         | Type              | Description                                                                                                             |
|------------------------------|-------------------|-------------------------------------------------------------------------------------------------------------------------|
| `RegistryAPI__ServiceUrl`    | URL               | The Registry API endpoint BCBCTI will connect to.                                                                       |
| `RegistryAPI__ApiKey`        | String            | The API key to authenticate with the Registry API. Required when using the Gateway Registry API endpoint.               |
| `RegistryAPI__Network`       | String (optional) | The blockchain network identifier used by the Registry API.                                                             |
| `RegistryAPI__IssuerDomain`  | String (optional) | The issuer domain used by the Registry API.                                                                             |
| `RegistryAPI__WebhookSecret` | String (optional) | A secret value to verify incoming webhooks. See [Webhooks](#webhooks) section below.                                    |
| `RegistryAPI__Mock`          | Boolean           | Makes BCBCTI mock Registry API calls when set to true. Can be used to test the BCBCTI server without consuming credits. |

Example Docker Compose configurations can be found in [examples/](./examples).

> [!IMPORTANT]
> If you use the [DCAP.Web](https://github.com/compellio/DCAP.Web/pkgs/container/dcap-web) as your Registry API instance, you must also set `BCBCTI__Ingestion__MaxConcurrentIngestions=1` in your environment variables, as DCAP.Web does not support concurrent registrations.

### Storage Providers

BCBCTI must be configured with a storage provider to store the STIX payloads it receives, and manage its internal state.

When registering STIX objects, BCBCTI generates a public URL pointing to the submitted STIX objects, to enable authorised users to access them, whether publicly or privately. This URL is generated based on the selected storage provider.

| Environment Variable        | Type | Description                                                                            |
|-----------------------------|------|----------------------------------------------------------------------------------------|
| `BCBCTI__Storage__Provider` | Enum | The selected storage provider. Must be one of the values listed in the sections below. |

#### File System

`BCBCTI__Storage__Provider` value must be set to `FileSystem`.

This is the default storage provider used in the container image.

The File System storage provider requires you to set up your own sharing mechanism (e.g. via an FTP server, WebDAV etc.) for the `{BCBCTI__Storage__FileSystem__BasePath}/objects` path.

| Environment Variable                         | Type   | Description                                                                                                                          |
|----------------------------------------------|--------|--------------------------------------------------------------------------------------------------------------------------------------|
| `BCBCTI__Storage__FileSystem__BasePath`      | String | The path to the folder to be used by BCBCTI to store STIX objects and metadata. The default container image uses `/data` by default. |
| `BCBCTI__Storage__FileSystem__PublicBaseUrl` | URL    | A public URL from which authorised users can access the items stored in the specified BasePath.                                      |

<details>

<summary>Example uses of BCBCTI__Storage__FileSystem__PublicBaseUrl</summary>

| BCBCTI__Storage__FileSystem__PublicBaseUrl | Generated STIX Object URL                                                                    |
|--------------------------------------------|----------------------------------------------------------------------------------------------|
| <https://stix.example.com/>                | <https://stix.example.com:443/objects/AzPqN9Inkfq5FB4j6Z_ET4UJRUERdzGwNe_9qji9Tio.json>      |
| <sftp://stix.example.com:22/bcbcti/>       | <sftp://stix.example.com:22/bcbcti/objects/AzPqN9Inkfq5FB4j6Z_ET4UJRUERdzGwNe_9qji9Tio.json> |

</details>

[//]: # (TODO IMPORTANT add note on publicly accessible/unique object key identifiers)

#### AWS S3, or S3-compatible services

`BCBCTI__Storage__Provider` value must be set to `S3`.

| Environment Variable                 | Type           | Description                                                                                                       |
|--------------------------------------|----------------|-------------------------------------------------------------------------------------------------------------------|
| `BCBCTI__Storage__S3__BucketName`    | String         | The S3 bucket name to be used by BCBCTI to store STIX objects and metadata.                                       |
| `BCBCTI__Storage__S3__PublicBaseUrl` | URL (optional) | Overrides the default AWS S3 public object URL. Must be defined when using S3-compatible services instead of AWS. |

[//]: # (TODO #### Azure Blob)

### Advanced

[//]: # (TODO collections variables)

## Webhooks

### Registry API (optional)

[//]: # (TODO while no auth, must expose /hooks path to the registry API + secret)

## Production

[//]: # (TODO notes on prod environments, incl. public object URIs AND[IF SELF-HOSTING-REGISTRY] public Registry API endpoint)
