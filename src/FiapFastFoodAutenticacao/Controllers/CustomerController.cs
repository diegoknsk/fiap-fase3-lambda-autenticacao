using Microsoft.AspNetCore.Mvc;
using FiapFastFoodAutenticacao.Core.UseCases;
using FiapFastFoodAutenticacao.Dtos;

namespace FiapFastFoodAutenticacao.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly CustomerIdentifyUseCase _identifyUseCase;
    private readonly CustomerRegisterUseCase _registerUseCase;
    private readonly CustomerRegisterAnonymousUseCase _registerAnonymousUseCase;

    public CustomerController(
        CustomerIdentifyUseCase identifyUseCase,
        CustomerRegisterUseCase registerUseCase,
        CustomerRegisterAnonymousUseCase registerAnonymousUseCase)
    {
        _identifyUseCase = identifyUseCase;
        _registerUseCase = registerUseCase;
        _registerAnonymousUseCase = registerAnonymousUseCase;
    }

    [HttpPost("identify")]
    public async Task<ActionResult<ApiResponse<CustomerTokenResponseModel>>> Identify([FromBody] CustomerIdentifyModel request)
    {
        try
        {
            var result = await _identifyUseCase.ExecuteAsync(request.Cpf);
            return Ok(ApiResponse<CustomerTokenResponseModel>.Ok(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<CustomerTokenResponseModel>.Error(ex.Message));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<CustomerTokenResponseModel>.Error("Erro interno do servidor"));
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<CustomerTokenResponseModel>>> Register([FromBody] CustomerRegisterModel request)
    {
        try
        {
            var result = await _registerUseCase.ExecuteAsync(request);
            return Ok(ApiResponse<CustomerTokenResponseModel>.Ok(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<CustomerTokenResponseModel>.Error("Erro interno do servidor"));
        }
    }

    [HttpPost("anonymous")]
    public async Task<ActionResult<ApiResponse<CustomerTokenResponseModel>>> RegisterAnonymous()
    {
        try
        {
            var result = await _registerAnonymousUseCase.ExecuteAsync();
            return Ok(ApiResponse<CustomerTokenResponseModel>.Ok(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<CustomerTokenResponseModel>.Error("Erro interno do servidor"));
        }
    }
}
