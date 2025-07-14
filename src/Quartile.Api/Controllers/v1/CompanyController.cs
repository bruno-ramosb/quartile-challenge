using MediatR;
using Microsoft.AspNetCore.Mvc;
using Quartile.Api.Helper;
using Quartile.Application.Features.Company.Commands;
using Quartile.Application.Dtos;

namespace Quartile.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CompanyController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<CompanyDto>> Create([FromBody] CreateCompanyCommand command)
        {
            var result = await mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CompanyDto>> GetById(Guid id)
        {
            var query = new GetCompanyByIdQuery(id);
            var result = await mediator.Send(query);
            return result.ToActionResult();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAll()
        {
            var query = new GetAllCompaniesQuery();
            var result = await mediator.Send(query);
            return result.ToActionResult();
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<CompanyDto>> Update(Guid id, [FromBody] UpdateCompanyCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Id in URL does not match Id in body");
            }

            var result = await mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var command = new DeleteCompanyCommand(id);
            var result = await mediator.Send(command);
            return result.ToActionResult();
        }
    }
}
