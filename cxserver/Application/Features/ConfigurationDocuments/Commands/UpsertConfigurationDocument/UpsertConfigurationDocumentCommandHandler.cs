using cxserver.Application.Abstractions;
using MediatR;

namespace cxserver.Application.Features.ConfigurationDocuments.Commands.UpsertConfigurationDocument;

internal sealed class UpsertConfigurationDocumentCommandHandler : IRequestHandler<UpsertConfigurationDocumentCommand>
{
    private readonly IConfigurationDocumentStore _store;
    private readonly IUnitOfWork _unitOfWork;

    public UpsertConfigurationDocumentCommandHandler(IConfigurationDocumentStore store, IUnitOfWork unitOfWork)
    {
        _store = store;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpsertConfigurationDocumentCommand request, CancellationToken cancellationToken)
    {
        await _store.UpsertAsync(request.NamespaceKey, request.DocumentKey, request.Payload, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
