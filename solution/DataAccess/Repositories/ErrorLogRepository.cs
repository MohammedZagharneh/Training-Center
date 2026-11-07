using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class ErrorLogRepository
    {
        private readonly TrainingCenterContext _context;

        public ErrorLogRepository(TrainingCenterContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ErrorLog errorLog)
        {
            _context.ErrorLogs.Add(errorLog);
            await _context.SaveChangesAsync();
        
        }
    }
}
