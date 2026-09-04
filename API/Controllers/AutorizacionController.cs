using API.Data;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AutorizacionController : ControllerBase
    {
        private readonly DataContext _dataContext;
/*
        public AutorizacionController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login(LoginDtoInput loginDtoInput)
        {
            
        }
        */
    }
}
