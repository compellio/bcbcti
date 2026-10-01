// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Extensions;

public static class DateTimeExtensions
{
    extension(DateTime date)
    {
        /// <summary>
        /// Converts the value of the current DateTime object to its equivalent TAXII timestamp representation (RFC 3339, with microsecond precision, in UTC with Z designation).
        /// </summary>
        /// <see href="https://docs.oasis-open.org/cti/taxii/v2.1/os/taxii-v2.1-os.html#_Toc26285787"/>
        public string ToTaxii()
        {
            return date.ToUniversalTime().ToString("yyy-MM-dd'T'HH:mm:ss.ffffffK");
        }
    }
}