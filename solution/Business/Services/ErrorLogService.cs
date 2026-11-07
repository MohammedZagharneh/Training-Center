using DataAccess.Entities;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class ErrorLogService
    {
        private readonly ErrorLogRepository _repository;

        public ErrorLogService(ErrorLogRepository repository)
        {
            _repository = repository;
        }

        public async Task LogAsync(Exception exception, string? requestPath, string? httpMethod)
        {
            var errorlog = new ErrorLog
            {
                Message = exception.Message.Length > 2000 ? exception.Message[..2000] : exception.Message,
                ExceptionType = exception.GetType().FullName ?? exception.GetType().Name,
                StackTrace = exception.StackTrace,
                RequestPath = requestPath,
                HttpMethod = httpMethod
            };

            await _repository.AddAsync(errorlog);
        }
    }
}
