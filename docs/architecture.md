# Architecture

This document describes how the BCBCTI server functions.

## Overview

The BCBCTI server is built around Compellio Gateway's [Registry API](https://docs.compellio.com/registry/) to register STIX Objects as Tokenized Asset Records (TARs).

```mermaid
%%TODO
```

[//]: # (TODO quick summary of TARs ~= TARs are verifiable tokens and have json payloads)

## Data Flow

When new STIX objects are submitted, the server decides whether to create a new TAR for the object, update the existing TAR associated with the object, or reject it.

This is a high-level overview of how an object is processed:

```mermaid
flowchart LR
    %% STIX Object handling flow

    classDef failure stroke:red
    classDef success stroke:green

    Request(["STIX Object <br/>Received"])

    Duplicate?{"Duplicate <br/>Object?"}
    Pending?{"Registration <br/>Pending?"}
    
    RejectDuplicate(["Reject Duplicate <br/>STIX Object"]):::failure
    RejectPending(["Reject Pending <br/>STIX Object"]):::failure
    Failure(["Registration <br/>Failed"]):::failure
    Success(["Add Object <br/>to Collection"]):::success
    
    Store["Store <br/>STIX Object"]
    Artifact["Build STIX <br/>Artifact Object"]
    
    Confirmed?{"Registration<br/>Completed?"}
 
    subgraph Registry API instance
        Register["Register TAR"]
    end
    
    Request --> Duplicate?
        Duplicate? -->|Yes| RejectDuplicate
        Duplicate? -->|No| Pending?
            Pending? -->|Yes| RejectPending
            Pending? -->|No| Store
                Store --> Artifact
                Artifact --> Register
    
    Register -.-> Confirmed?
    
    Confirmed? -->|Yes|Success
    Confirmed? -->|No / Timeout|Failure
```

Submitted objects will only appear in their collection once they have been properly registered by the Registry API.

A limitation of the current BCBCTI version is that STIX Object modifications will be rejected (_Reject Pending STIX Object_) until the previously submitted version gets registered by the Registry API.

A rejected or failed submission can be resubmitted by the client, but BCBCTI does not retry on the client's behalf.
