using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Domain.Entities
{
    public class ProviderOrderItem : EntityBase
    {
        public Guid ProviderOrderId { get; private set; }

        public Guid? DtpOrderItemId { get; private set; }

        public int? ProviderProductId { get; private set; }

        public string Sku { get; private set; } = default!;

        public string ProductName { get; private set; } = default!;

        public int Qty { get; private set; }

        public string RawSerialsJson { get; private set; } = default!;


        public ProviderOrder? ProviderOrder { get; private set; }

        private readonly List<ProviderRedeem> _redeems = new();
        public IReadOnlyCollection<ProviderRedeem> Redeems => _redeems.AsReadOnly();

        private ProviderOrderItem()
        {
        }

        public ProviderOrderItem(
            Guid providerOrderId,
            Guid? dtpOrderItemId,
            int? providerProductId,
            string sku,
            string productName,
            int qty,
            string rawSerialsJson)
        {
            ProviderOrderId = providerOrderId;
            DtpOrderItemId = dtpOrderItemId;
            ProviderProductId = providerProductId;
            Sku = sku;
            ProductName = productName;
            Qty = qty;
            RawSerialsJson = rawSerialsJson;
        }
    }
}
