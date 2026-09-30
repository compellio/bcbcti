// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Services.Storage.Models;

// see https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/Conditional_requests
// see https://docs.aws.amazon.com/AmazonS3/latest/userguide/conditional-writes.html
public class Condition
{
    public enum Kind
    {
        None,
        /// <summary>
        /// Stores an object only if the object key does not already exist in the store.
        /// </summary>
        IfNotExists,
        /// <summary>
        /// Stores an object only if the ETag (entity tag) matches the ETag of the object in the store.
        /// </summary>
        IfMatch
    }

    public Kind Type { get; init; }
    public string? ETag { get; }

    private Condition(Kind type, string? etag)
    {
        Type = type;
        ETag = etag;
    }

    public static readonly Condition None = new(Kind.None, null);
    public static readonly Condition IfNoneExists = new(Kind.IfNotExists, null);
    public static Condition IfMatch(string etag) => new(Kind.IfMatch, etag);
}