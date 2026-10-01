// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Extensions;

// see https://stackoverflow.com/a/75476487
// TODO-REVIEW
internal static class EnumerableExtensions
{
    extension<T>(IEnumerable<T?> enumerable) where T : struct
    {
        public IEnumerable<T> WhereNotNull()
        {
            return enumerable.Where(item => item.HasValue).Select(item => item!.Value);
        }
    }

    extension<T>(IEnumerable<T?> enumerable) where T : class
    {
        public IEnumerable<T> WhereNotNull()
        {
            return enumerable.Where(item => item is not null).Select(item => item!);
        }
    }
}