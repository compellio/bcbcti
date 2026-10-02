// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Repositories;

namespace Compellio.Bcbcti.Services.Ingestion;

public class RegistryOperationsManager
{
    private readonly RegistryOperationRepository _registryOperationRepository;
    private readonly RegistrationReceiptsRepository _registrationReceiptsRepository;
    private readonly StixObjectRepository _stixObjectRepository;

    public RegistryOperationsManager(RegistryOperationRepository registryOperationRepository,
        RegistrationReceiptsRepository registrationReceiptsRepository, StixObjectRepository stixObjectRepository)
    {
        _registryOperationRepository = registryOperationRepository;
        _registrationReceiptsRepository = registrationReceiptsRepository;
        _stixObjectRepository = stixObjectRepository;
    }

    public Task Complete(RegistryOperation operation, CancellationToken ct = default)
    {
        return _registryOperationRepository.DeleteRegistryOperation(operation.ObjectId, ct);
    }

    public async Task Abandon(RegistryOperation operation, DateTime completedAt, string reason, CancellationToken ct = default)
    {
        if (operation.WasSent)
        {
            var receipt = await _registrationReceiptsRepository.GetReceipt(operation.ReceiptId.Value, ct);
            await _registrationReceiptsRepository.UpdateReceipt(receipt.Metadata.ETag, receipt.Body.AsFailed(completedAt, reason), ct);
        }

        if (operation.HasObject)
        {
            await _stixObjectRepository.DeleteStixObject(operation.ObjectKey, ct);
        }

        await Complete(operation, ct);
    }
}