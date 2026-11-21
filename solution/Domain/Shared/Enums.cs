using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shared
{
    public enum GetDetailsStatus
    {
        Found, NotFound, InvalidInput
    }

    public enum DeleteResult
    {
        Deleted, NotFound, InvalidInput, ForeignKeyViolation
    }

    public enum GetListStatus
    {
        InvalidPaginationInputs, Success
    }
}
