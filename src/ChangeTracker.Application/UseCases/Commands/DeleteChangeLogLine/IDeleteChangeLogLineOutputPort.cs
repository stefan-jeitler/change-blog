using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Commands.DeleteChangeLogLine
{
    public interface IDeleteChangeLogLineOutputPort
    {
        public void LineDoesNotExist();
        void LineDeleted(Guid lineId);
        void Conflict(string reason);
    }
}
