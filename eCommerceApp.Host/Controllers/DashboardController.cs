using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eCommerceApp.Application.Features.Dashboard.Queries;

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

        /// <summary>
        /// Gets the product and inventory dashboard data.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An object containing dashboard metrics.</returns>
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
    }
}
