using Domain.Shared.Errors;
using Domain.Shared.Results;

namespace Domain.Shared.Entities;

public abstract class EntityDeletable : Entity
{
    public bool IsDeleted { get; private set; } = false;

    public Result Delete()
    {
        if (IsDeleted)
        {
            return EntityDeletableErrors.AlreadyDeleted;
        }

        IsDeleted = true;

        return Result.Ok();
    }

    public Result Restore()
    {
        if (!IsDeleted)
        {
            return EntityDeletableErrors.NotDeleted;
        }

        IsDeleted = false;

        return Result.Ok();
    }
}