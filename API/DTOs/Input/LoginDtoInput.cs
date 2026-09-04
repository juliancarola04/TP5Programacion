using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public record LoginDtoInput(
    string Username,
    string Password);