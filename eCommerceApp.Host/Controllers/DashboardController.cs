using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eCommerceApp.Application.Features.Dashboard.Queries;
using MediatR;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

       
        [HttpGet("product-inventory")]
        [ProducesResponseType(typeof(eCommerceApp.Application.DTOs.Dashboard.ProductInventoryDashboardDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetProductInventoryDashboard(CancellationToken cancellationToken)
        {
            var query = new GetProductInventoryDashboardQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("sales-analytics")]
        [ProducesResponseType(typeof(eCommerceApp.Application.DTOs.Dashboard.SalesAnalyticsDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetSalesAnalytics([FromQuery] string period, CancellationToken cancellationToken)
        {
            var query = new GetSalesAnalyticsQuery { Period = period };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("order-analytics")]
        [ProducesResponseType(typeof(eCommerceApp.Application.DTOs.Dashboard.OrderAnalyticsDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetOrderAnalytics(CancellationToken cancellationToken)
        {
            var query = new GetOrderAnalyticsQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("customer-analytics")]
        [ProducesResponseType(typeof(eCommerceApp.Application.DTOs.Dashboard.OrderAnalyticsDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetCustomerAnalytics(CancellationToken cancellationToken)
        {
            var query = new GetCustomerAnalyticsQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

    }
}
