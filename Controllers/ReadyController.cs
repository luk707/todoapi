using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("api/v1/ready")]
public class ReadyController : ControllerBase
{
    /// <summary>
    /// Readiness check endpoint.
    /// </summary>
    /// <remarks>
    /// This API is used to check if the service is ready to handle requests.
    /// </remarks>
    /// <response code="204">Service is ready</response>
    /// <response code="500">Service is not ready</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Check service readiness",
        Description = "Returns 204 if the service is ready, otherwise returns an error."
    )]
    [SwaggerResponse(204, "Service is ready")]
    [SwaggerResponse(500, "Service is not ready")]
    public IActionResult GetHello()
    {
        return NoContent();
    }
}
