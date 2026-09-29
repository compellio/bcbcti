# Configuration

[//]: # (TODO)

> [!CAUTION]
> BCBCTI is meant to be deployed as an internal service and **does not implement authentication**. Anything that can reach a collection endpoint can write to it. If you plan to deploy BCBCTI publicly, you should place it behind your own authentication layer (e.g. reverse proxy, API gateway, etc.).

## Environment Variables

[//]: # (TODO common env variables, essentially everything under BCBCTI:)

### Registry API

[//]: # (TODO registry api envs)

### Storage Providers

#### Local Disk

[//]: # (TODO IMPORTANT add note on publicly accessible/unique object key identifiers)

#### AWS S3, or S3-compatible services

#### Azure Blob

## Webhooks

### Registry API (optional)

[//]: # (TODO while no auth, must expose /hooks path to the registry API + secret)

## Production

[//]: # (TODO notes on prod environments, incl. public object URIs AND[IF SELF-HOSTING-REGISTRY] public Registry API endpoint)
