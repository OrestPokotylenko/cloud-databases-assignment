using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.Products.ConfirmProductImageUpload;

public record ConfirmProductImageUploadCommand(
    Guid ProductPublicId,
    Guid ImagePublicId)
    : ICommand<ConfirmProductImageUploadResult>;
