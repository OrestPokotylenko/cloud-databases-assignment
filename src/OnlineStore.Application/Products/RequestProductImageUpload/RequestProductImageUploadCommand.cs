using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.Products.RequestProductImageUpload;

public record RequestProductImageUploadCommand(
    Guid ProductPublicId,
    string ContentType,
    long SizeBytes)
    : ICommand<RequestProductImageUploadResult>;
