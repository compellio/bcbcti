# Architecture

This document describes how the BCBCTI server functions.

## Overview

The BCBCTI server is built around Compellio Gateway's [Registry API](https://docs.compellio.com/registry/) to register STIX Objects as Tokenized Asset Records (TARs).



[//]: # (TODO quick summary of TARs ~= TARs are verifiable tokens and have json payloads)

When new STIX objects are submitted, the server decides whether to create a new TAR for the object, update the existing TAR associated with the object, or reject it.

