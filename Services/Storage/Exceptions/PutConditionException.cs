// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Services.Storage.Exceptions;

public class PutConditionException(string message, Exception? innerException = null)
    : ProviderOperationException(message, innerException);