using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class ErrorLog
    {
        public long ErrorLogID { get; set; }

        public string Message { get; set; } = null!;

        public string ExceptionType { get; set; } = null!;

        public string? StackTrace { get; set; }

        public string? RequestPath { get; set; }

        public string? HttpMethod { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
