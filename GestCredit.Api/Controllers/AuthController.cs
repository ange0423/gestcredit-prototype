using FluentValidation;
using FluentValidation.Results;
using GestCredit.Api.Dtos;
using GestCredit.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestCredit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterDto> _registerValidator;

    public AuthController(IAuthService authService, IValidator<RegisterDto> registerValidator)
    {
        _authService = authService;
        _registerValidator = registerValidator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        ValidationResult validation = await _registerValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            foreach (ValidationFailure error in validation.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            return ValidationProblem(ModelState);
        }

        AuthResponseDto? result = await _authService.RegisterAsync(dto);
        if (result is null) return BadRequest("Nom d'utilisateur déjà pris, ou rôle inconnu.");

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        AuthResponseDto? result = await _authService.LoginAsync(dto);
        if (result is null) return Unauthorized("Identifiants invalides.");

        return Ok(result);
    }
}
