# Configuration

[//]: # (TODO common env variables, essentially everything under BCBCTI:)

## Registry API

[//]: # (TODO registry api envs)

## Storage Providers

BCBCTI must be configured with a storage provider to store the STIX payloads it receives, and manage its internal state.

When registering STIX objects, BCBCTI generates a public URL pointing to the submitted STIX objects, to enable authorised users to access them, whether publicly or privately. This URL is generated based on the selected storage provider.

| Environment Variable        | Type | Description                                                                            |
|-----------------------------|------|----------------------------------------------------------------------------------------|
| `BCBCTI__Storage__Provider` | Enum | The selected storage provider. Must be one of the values listed in the sections below. |

### File System

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

### AWS S3, or S3-compatible services

`BCBCTI__Storage__Provider` value must be set to `S3`.

| Environment Variable                 | Type           | Description                                                                                                       |
|--------------------------------------|----------------|-------------------------------------------------------------------------------------------------------------------|
| `BCBCTI__Storage__S3__BucketName`    | String         | The S3 bucket name to be used by BCBCTI to store STIX objects and metadata.                                       |
| `BCBCTI__Storage__S3__PublicBaseUrl` | URL (optional) | Overrides the default AWS S3 public object URL. Must be defined when using S3-compatible services instead of AWS. |

If BCBCTI is running on AWS, we recommend to use an IAM role on the BCBCTI container to access S3.
Otherwise, you can create an IAM user and generate an Access Key pair for BCBCTI (set the `AWS_ACCESS_KEY_ID` and `AWS_SECRET_ACCESS_KEY` environment variables).

In both cases, apply the following policy to the IAM entity to grant BCBCTI the necessary permissions to access the bucket:

```json
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Sid": "BcbctiBucketAccess",
            "Effect": "Allow",
            "Action": ["s3:PutObject", "s3:GetObject", "s3:ListBucket", "s3:DeleteObject"],
            "Resource": [
                "arn:aws:s3:::{bucket_name}",
                "arn:aws:s3:::{bucket_name}/*"
            ]
        }
    ]
}
```

The server can also connect to AWS using temporary credentials (set the `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, and `AWS_SESSION_TOKEN` environment variables).

#### S3-compatible services

BCBCTI can work with S3-compatible interfaces that support the following S3 operations: GetObject, PutObject (with support for If-None-Match and If-Match conditions), ListObjectsV2, and DeleteObject.

When working with an S3-compatible service, you will need to set the `AWS_ENDPOINT_URL_S3`, and potentially the `AWS_FORCE_PATH_STYLE` variables.

Below is an example configuration that connects to the Hetzner Object Storage service:

```shell
AWS_ENDPOINT_URL_S3=https://nbg1.your-objectstorage.com

AWS_ACCESS_KEY_ID={hetzner_credential_id}
AWS_SECRET_ACCESS_KEY={hetzner_credential_secret}

BCBCTI__Storage__S3__BucketName={bucket_name}
```

BCBCTI will attempt to derive public object URLs for the `/objects` path automatically, but you may need to use the `BCBCTI__Storage__S3__PublicBaseUrl` variable to adjust it manually if the generated URLs are invalid.

### Azure Blob

[//]: # (TODO #### Azure Blob)

## Advanced

## Webhooks

### Registry API (optional)

[//]: # (TODO while no auth, must expose /hooks path to the registry API + secret)

[//]: # (TODO section with notes on "stg/prod" environments, incl. public object URIs AND[IF SELF-HOSTING-REGISTRY] public Registry API endpoint)
