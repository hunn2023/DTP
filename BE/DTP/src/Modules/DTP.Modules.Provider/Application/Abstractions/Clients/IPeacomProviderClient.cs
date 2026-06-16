using DTP.Modules.Provider.Application.DTOs;
using DTP.Modules.Provider.Application.DTOs.Peacoms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Clients
{
    public interface IPeacomProviderClient
    {

        Task<IReadOnlyList<ProviderPackageProductRemoteDto>> GetPackageProductsAsync(
          Domain.Entities.Provider provider,
          CancellationToken cancellationToken = default);

        Task<ProviderEsimProductRemoteDto> GetProductEsimAsync(
            Domain.Entities.Provider provider,
            string sku,
            CancellationToken cancellationToken = default);


        Task<PeacomCreateOrderResponse> CreateOrderAsync(
              Domain.Entities.Provider provider,
              PeacomCreateOrderRequest request,
              CancellationToken cancellationToken = default);

        Task<PeacomConfirmOrderResponse> ConfirmOrderAsync(
            string publicId,
            bool isConfirm,
            CancellationToken cancellationToken = default);

        Task<PeacomRedeemResponse> RedeemAsync(
            PeacomRedeemRequest request,
            CancellationToken cancellationToken = default);

        Task<PeacomRedeemInfoResponse> GetRedeemInfoAsync(
            string serial,
            CancellationToken cancellationToken = default);
    }
}
