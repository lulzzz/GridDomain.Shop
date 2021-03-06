﻿using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Tests.Common;
using Moq;
using Shop.Domain.Aggregates.UserAggregate;
using Shop.Domain.Aggregates.UserAggregate.Commands;
using Shop.Domain.Aggregates.UserAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.UserAggregate
{
    public class User_created_tests
    {
        private readonly Guid _stockId = Guid.NewGuid();

        private AggregateScenario<User> NewScenario()
        {
            return AggregateScenario.New(new UserCommandsHandler());
        }

        [Fact]
        public async Task Given_user_created_When_buing_now_Then_pending_order_event_is_created()
        {
            var userId = Guid.NewGuid();
            var skuId = Guid.NewGuid();
            var quantity = 10;
            var account = Guid.NewGuid();

            var scenario = await NewScenario()
                               .Given(new UserCreated(userId, "testLogin", account))
                               .When(new BuySkuNowCommand(userId, skuId, quantity, _stockId))
                               .Then(new SkuPurchaseOrdered(userId, skuId, 10, Any.GUID, _stockId, account))
                               .Run().Check();

            //Aggregate state 
            var pendingOrderId = scenario.ProducedEvents.OfType<SkuPurchaseOrdered>().First().OrderId;
            var pendingOrder = scenario.Aggregate.PendingOrders[pendingOrderId];
            Assert.Equal(quantity, pendingOrder.Quantity);
            Assert.Equal(skuId, pendingOrder.SkuId);
            Assert.Equal(pendingOrderId, pendingOrder.Order);
        }

        [Fact]
        public async Task Given_user_with_pending_order_When_cancel_order_Then_order_canceled_is_created()
        {
            var userId = Guid.NewGuid();
            var skuId = Guid.NewGuid();
            var quantity = 10;
            var orderId = Guid.NewGuid();
            var account = Guid.NewGuid();


            var scenario = await NewScenario()
                                 .Given(new UserCreated(userId, "testLogin", account),
                                        new SkuPurchaseOrdered(userId, skuId, quantity, orderId, _stockId, account))
                                 .When(new CancelPendingOrderCommand(userId, orderId))
                                 .Then(new PendingOrderCanceled(userId, orderId))
                                 .Run()
                                 .Check();

            //pending order should be removed
            Assert.Empty(scenario.Aggregate.PendingOrders);
        }

        [Fact]
        public async Task Given_user_with_pending_order_When_complete_order_Then_order_completed_is_emitted()
        {
            var userId = Guid.NewGuid();
            var skuId = Guid.NewGuid();
            var quantity = 10;
            var orderId = Guid.NewGuid();
            var account = Guid.NewGuid();


            var scenario =
                await NewScenario()
                    .Given(new UserCreated(userId, "testLogin", account),
                           new SkuPurchaseOrdered(userId, skuId, quantity, orderId, _stockId, account))
                    .When(new CompletePendingOrderCommand(userId, orderId))
                    .Then(new PendingOrderCompleted(userId, orderId))
                    .Run()
                    .Check();

            //pending order should be removed
            Assert.Empty(scenario.Aggregate.PendingOrders);
        }

        [Fact]
        public async Task When_user_created_by_command_Then_created_event_occures()
        {
            var cmd = new CreateUserCommand(Guid.NewGuid(), "testLogin", Guid.NewGuid());

            var scenario = await NewScenario()
                               .When(cmd)
                               .Then(new UserCreated(cmd.UserId, cmd.Login, cmd.AccountId))
                               .Run()
                               .Check();

            Assert.Equal(cmd.Login, scenario.Aggregate.Login);
            Assert.Equal(cmd.AccountId, scenario.Aggregate.Account);
            Assert.Equal(cmd.UserId, scenario.Aggregate.Id);
        }
    }
}