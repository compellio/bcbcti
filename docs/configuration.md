# Configuration

[//]: # (TODO)

> [!CAUTION]
> BCBCTI is meant to be deployed as an internal service and **does not implement
authentication**. Anything that can reach a collection endpoint can write to it. If you plan to deploy BCBCTI publicly, you should place it behind your own authentication layer (e.g. reverse proxy, API gateway, etc.).

## Environment Variables

[//]: # (TODO common env variables, essentially everything under BCBCTI:)

### Registry API

[//]: # (TODO registry api envs)

### Storage Providers

| Environment Variable        | Type | Description                                                                            |
|-----------------------------|------|----------------------------------------------------------------------------------------|
| `BCBCTI__Storage__Provider` | Enum | The selected storage provider. Must be one of the values listed in the sections below. |

#### File System

`BCBCTI__Storage__Provider` value must be set to `FileSystem`.

This is the default storage provider used in the container image.

| Environment Variable                    | Type   | Description                                                                                                                          |
|-----------------------------------------|--------|--------------------------------------------------------------------------------------------------------------------------------------|
| `BCBCTI__Storage__FileSystem__BasePath` | String | The path to the folder to be used by BCBCTI to store STIX objects and metadata. The default container image uses `/data` by default. |
| `BCBCTI__Storage__FileSystem__BaseUri`  | URL    | A public URL from which authorised users can access the items stored in the specified BasePath.                                      |

[//]: # (TODO IMPORTANT add note on publicly accessible/unique object key identifiers)

#### AWS S3, or S3-compatible services

`BCBCTI__Storage__Provider` value must be set to `S3`.

| Environment Variable                 | Type           | Description                                                                                                       |
|--------------------------------------|----------------|-------------------------------------------------------------------------------------------------------------------|
| `BCBCTI__Storage__S3__BucketName`    | String         | The S3 bucket name to be used by BCBCTI to store STIX objects and metadata.                                       |
| `BCBCTI__Storage__S3__PublicBaseUrl` | URL (optional) | Overrides the default AWS S3 public object URL. Must be defined when using S3-compatible services instead of AWS. |

[//]: # (TODO #### Azure Blob)

## Webhooks

### Registry API (optional)

[//]: # (TODO while no auth, must expose /hooks path to the registry API + secret)

## Production

[//]: # (TODO notes on prod environments, incl. public object URIs AND[IF SELF-HOSTING-REGISTRY] public Registry API endpoint)
