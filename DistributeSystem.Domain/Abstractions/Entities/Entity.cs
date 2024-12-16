using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributeSystem.Domain.Abstractions.Entities
{
    public abstract class Entity<T> : IEntity<T>
    {
        public T Id { get; protected set; }
        public bool IsDeleted { get; protected set; }
    }
}
