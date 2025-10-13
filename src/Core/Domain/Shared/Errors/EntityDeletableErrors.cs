using Domain.Shared.Results;

namespace Domain.Shared.Errors;

public static class EntityDeletableErrors
{
    public static readonly Error AlreadyDeleted =
       Error.Problem("entityDeletable.Delete", "The register is already deleted.");
    
    public static readonly Error NotDeleted = 
        Error.Problem("entityDeletable.Delete", "The register is not deleted.");
}