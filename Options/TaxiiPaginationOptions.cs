// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Options;

public class TaxiiPaginationOptions
{
    public int MaxLimit { get; set; } = 50;
    
    public int DefaultLimit { get; set; } = 25;
}