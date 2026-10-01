// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Models.Taxii;

/// <summary>
/// TAXII 5.2.1 Collections Resource
/// </summary>
/// <see href="https://docs.oasis-open.org/cti/taxii/v2.1/os/taxii-v2.1-os.html#_Toc26285809"/>
public class CollectionsResource
{
    /// <summary>
    /// The id property universally and uniquely identifies this Collection. It is used in the Get Collection Endpoint (see section 5.2) as the {id} parameter to retrieve the Collection.
    /// </summary>
    public required CollectionResource[]? Collections { get; set; }

}