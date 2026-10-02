// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Compellio.Bcbcti.Models.Taxii;
using Compellio.Bcbcti.Options;
using Microsoft.Extensions.Options;

namespace Compellio.Bcbcti.Services.Taxii;

public class TaxiiServerService
{
    private readonly IOptions<TaxiiOptions> _options;

    public TaxiiServerService(IOptions<TaxiiOptions> options)
    {
        _options = options;
    }

    public DiscoveryResource BuildDiscoveryResource()
    {
        // TODO FIXME the /api/ TAXII {api-root} is hardcoded in both controllers and TAXII services -> can drift
        return new DiscoveryResource { Title = _options.Value.ServerTitle, Default = "/api/", ApiRoots = ["/api/"] };
    }

    public ApiRootResource BuildApiRootResource()
    {
        return new ApiRootResource
        {
            Title = _options.Value.Title,
            Description = _options.Value.Description,
            Versions = ["application/taxii+json;version=2.1"], // TODO move to constant
            MaxContentLength = (int)_options.Value.MaxUploadBytes,
        };
    }
}