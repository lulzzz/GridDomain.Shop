using GridDomain.CQRS;
using Microsoft.AspNetCore.Mvc;

namespace Shop.Web.Controllers.Domain {
    public class SkuStockController : Controller
    {
        private readonly ICommandExecutor _commandBus;

        public SkuStockController(ICommandExecutor commandBus)
        {
            _commandBus = commandBus;
        }
    }
}