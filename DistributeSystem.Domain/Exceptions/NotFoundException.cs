using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributeSystem.Domain.Exceptions
{
    public abstract class NotFoundException : DomainException
    {
        protected NotFoundException(string message) : base("Not Found", message) 
        {
        }
    }
}
