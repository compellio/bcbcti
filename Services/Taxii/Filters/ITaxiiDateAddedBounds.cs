// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Services.Taxii.Filters;

// TODO-REVIEW placement
public interface ITaxiiDateAddedBounds
{
    public DateTime? DateAddedFirst { get; }
    public DateTime? DateAddedLast { get; }
}