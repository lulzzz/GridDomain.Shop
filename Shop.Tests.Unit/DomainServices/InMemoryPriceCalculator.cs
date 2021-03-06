using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NMoneys;
using Shop.Domain.DomainServices.PriceCalculator;

namespace Shop.Tests.Unit.DomainServices
{
    public class InMemoryPriceCalculator : IPriceCalculator
    {
        private readonly IDictionary<Guid, Money> _skuPrices = new Dictionary<Guid, Money>();

        public Task<Money> CalculatePrice(Guid skuId, int quantity)
        {
            if (!_skuPrices.TryGetValue(skuId, out var skuPrice))
                throw new SkuPriceNotFoundException(skuId);

            return Task.FromResult(skuPrice * quantity);
        }

        public void Add(Guid skuId, Money money)
        {
            _skuPrices[skuId] = money;
        }
    }
}