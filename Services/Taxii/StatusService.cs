using Compellio.Bcbcti.Models.Taxii;
using Compellio.Bcbcti.Repositories;
using Compellio.Bcbcti.Services.Taxii.Mappers;

namespace Compellio.Bcbcti.Services.Taxii;

public class StatusService
{
    private readonly JournalRepository _journalRepository;
    private readonly RegistrationReceiptsRepository _receiptsRepository;

    public StatusService(JournalRepository journalRepository, RegistrationReceiptsRepository receiptsRepository)
    {
        _journalRepository = journalRepository;
        _receiptsRepository = receiptsRepository;
    }

    public async Task<StatusResource> BuildStatusResource(Guid id, CancellationToken ct = default)
    {
        var journal = await _journalRepository.GetJournalEntry(id, ct);

        var receiptIds = journal.Body.Objects.Select(o => o.ReceiptId).WhereNotNull().ToArray();
        var receipts = await _receiptsRepository.FindReceipts(receiptIds, ct);

        var receiptsDict = receipts.WhereNotNull().ToDictionary(o => o.Body.ReceiptId, o => o.Body);

        return StatusMapper.ToResource(journal.Body, receiptsDict);
    }
}