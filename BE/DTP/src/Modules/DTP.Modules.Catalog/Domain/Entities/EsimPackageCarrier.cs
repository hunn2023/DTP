using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Domain.Entities
{
    public class EsimPackageCarrier : EntityBase
    {
        private EsimPackageCarrier()
        {
        }

        public EsimPackageCarrier(
            Guid esimPackageId,
            Guid carrierId,
            bool isPrimary,
            int sortOrder)
        {
            Id = Guid.NewGuid();
            EsimPackageId = esimPackageId;
            CarrierId = carrierId;
            IsPrimary = isPrimary;
            SortOrder = sortOrder;
        }

        public Guid EsimPackageId { get; private set; }

        public EsimPackage EsimPackage { get; private set; } = default!;

        public Guid CarrierId { get; private set; }

        public Carrier Carrier { get; private set; } = default!;

        public bool IsPrimary { get; private set; }

        public int SortOrder { get; private set; }

        public void Update(bool isPrimary, int sortOrder)
        {
            IsPrimary = isPrimary;
            SortOrder = sortOrder;
        }
    }
}
