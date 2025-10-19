using Microsoft.AspNetCore.Mvc;
using GanhoDeCapital.Core.Services;
using GanhoDeCapital.Core.Validators;
using GanhoDeCapital.Core.Domain.DTOs;

namespace GanhoDeCapital.Api.Controllers;

[ApiController]
[Route("")]
public class TaxController : ControllerBase
{
    private readonly ProcessTaxService _processTaxService;
    private readonly OperationRequestValidator _validator;
    private readonly ILogger<TaxController> _logger;

    public TaxController(
        ProcessTaxService processTaxService,
        OperationRequestValidator validator,
        ILogger<TaxController> logger)
    {
        _processTaxService = processTaxService;
        _validator = validator;
        _logger = logger;
    }

    /// <summary>
    /// Processa lotes de operações e calcula impostos
    /// </summary>
    /// <param name="requests">Lista de operações para processar</param>
    /// <returns>Lista de resultados com impostos calculados</returns>
    [HttpPost("process-taxes")]
    [ProducesResponseType(typeof(List<OperationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ProcessTaxes([FromBody] List<OperationRequest> requests)
    {
        try
        {
            _logger.LogInformation(
                "Recebida requisição para processar {Count} operações",
                requests?.Count ?? 0);

            // Validação
            var validationErrors = _validator.Validate(requests);
            if (validationErrors.Any())
            {
                _logger.LogWarning("Erros de validação: {Errors}", string.Join("; ", validationErrors));
                return BadRequest(new { errors = validationErrors });
            }

            // Processamento
            var responses = await _processTaxService.ProcessOperationsAsync(requests);

            _logger.LogInformation(
                "Processamento concluído com sucesso para {Count} operações",
                responses.Count);

            return Ok(responses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar operações");
            return StatusCode(500, new { message = "Erro interno ao processar operações" });
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            service = "Ganho de Capital API"
        });
    }
}