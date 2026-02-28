using cxserver.Application.Abstractions;
using cxserver.Domain.ContactEngine;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.Contact.Commands.SubmitContactMessage;

public sealed record SubmitContactMessageCommand(
    string Name,
    string Email,
    string? Subject,
    string Message) : IRequest<SubmitContactMessageResponse>;

public sealed record SubmitContactMessageResponse(
    Guid Id,
    DateTimeOffset CreatedAtUtc);

internal sealed class SubmitContactMessageCommandHandler : IRequestHandler<SubmitContactMessageCommand, SubmitContactMessageResponse>
{
    private readonly IContactMessageStore _store;
    private readonly ITenantContext _tenantContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitContactMessageCommandHandler(
        IContactMessageStore store,
        ITenantContext tenantContext,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _store = store;
        _tenantContext = tenantContext;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<SubmitContactMessageResponse> Handle(SubmitContactMessageCommand request, CancellationToken cancellationToken)
    {
        if (_tenantContext.TenantId is null)
        {
            throw new InvalidOperationException("Tenant context is required.");
        }

        var now = _dateTimeProvider.UtcNow;
        var message = ContactMessage.Create(
            Guid.NewGuid(),
            _tenantContext.TenantId.Value,
            request.Name,
            request.Email,
            request.Subject,
            request.Message,
            now);

        await _store.AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SubmitContactMessageResponse(message.Id, message.CreatedAtUtc);
    }
}

internal sealed class SubmitContactMessageCommandValidator : AbstractValidator<SubmitContactMessageCommand>
{
    public SubmitContactMessageCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(320);

        RuleFor(x => x.Subject)
            .MaximumLength(512);

        RuleFor(x => x.Message)
            .NotEmpty()
            .MaximumLength(4000);
    }
}
