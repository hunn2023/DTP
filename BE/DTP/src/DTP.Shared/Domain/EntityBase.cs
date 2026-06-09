using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Shared.Domain
{
    public abstract class EntityBase
    {
        public Guid Id { get; protected set; }

        public DateTime CreatedAt { get; protected set; }

        public Guid? CreatedBy { get; protected set; }

        public DateTime? UpdatedAt { get; protected set; }

        public Guid? UpdatedBy { get; protected set; }

        public bool IsDeleted { get; protected set; }

        protected EntityBase()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            IsDeleted = false;
        }

        public virtual void SetUpdated(Guid? userId = null)
        {
            UpdatedAt = DateTime.Now;
            UpdatedBy = userId;
        }

        public virtual void Delete(Guid? userId = null)
        {
            IsDeleted = true;
            UpdatedAt = DateTime.Now;
            UpdatedBy = userId;
        }
    }
}
