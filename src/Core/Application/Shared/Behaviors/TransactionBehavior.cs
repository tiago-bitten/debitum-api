using Application.Shared.Messaging;
using Application.Shared.Ports;
using Domain.Shared.Results;

namespace Application.Shared.Behaviors;

internal static class TransactionDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        IUnitOfWork unitOfWork
    ) : ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            if (unitOfWork.HasActiveTransaction)
            {
                return await innerHandler.HandleAsync(command, cancellationToken);
            }

            await unitOfWork.BeginAsync(cancellationToken);

            try
            {
                var result = await innerHandler.HandleAsync(command, cancellationToken);

                if (result.IsSuccess)
                {
                    await unitOfWork.CommitAsync(cancellationToken);
                }
                else
                {
                    await unitOfWork.RollbackAsync(cancellationToken);
                }

                return result;
            }
            catch
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }

    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        IUnitOfWork unitOfWork
    ) : ICommandHandler<TCommand> where TCommand : ICommand
    {
        public async Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            if (unitOfWork.HasActiveTransaction)
            {
                return await innerHandler.HandleAsync(command, cancellationToken);
            }

            await unitOfWork.BeginAsync(cancellationToken);

            try
            {
                var result = await innerHandler.HandleAsync(command, cancellationToken);

                if (result.IsSuccess)
                {
                    await unitOfWork.CommitAsync(cancellationToken);
                }
                else
                {
                    await unitOfWork.RollbackAsync(cancellationToken);
                }

                return result;
            }
            catch
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}