// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Services.RegistryApi;

public class RegistryApiException(string? message, Exception? innerException = null) : Exception(message, innerException);
