using System;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.Creation;
using Serilog;
using Shop.Domain.Aggregates.UserAggregate.Events;
using Shop.Domain.DomainServices.PriceCalculator;

namespace Shop.Domain.ProcessManagers
{
    public class BuyNowProcessManagerFactory : IProcessManagerCreator<BuyNowState>,
                                     IProcessManagerCreator<BuyNowState,SkuPurchaseOrdered>
    {
        private readonly ILogger _log;
        private readonly IPriceCalculator _priceCalculator;

        public BuyNowProcessManagerFactory(IPriceCalculator priceCalculator, ILogger log)
        {
            _log = log;
            _priceCalculator = priceCalculator;
        }

        public IProcessManager<BuyNowState> Create(BuyNowState state)
        {
            return new ProcessManager<BuyNowState>(new BuyNow(_priceCalculator),state, _log);
        }

        public IProcessManager<BuyNowState> CreateNew(SkuPurchaseOrdered message, Guid? processId = null)
        {
            return Create(new BuyNowState(processId ?? Guid.NewGuid(), nameof(BuyNow.Initial)));
        }
    }
}