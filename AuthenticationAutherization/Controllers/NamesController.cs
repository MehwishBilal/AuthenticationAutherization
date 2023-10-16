using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthenticationAutherization.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NamesController : ControllerBase
    {
        public ICustomAuthenticationManager CustomAuthenticationManager { get; }

        public NamesController(ICustomAuthenticationManager customAuthenticationManager)
        {
            CustomAuthenticationManager = customAuthenticationManager;
        }

        // GET: api/<NamesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Mehiwsh", "Bilal  " };
        }

        // GET api/<NamesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate(UserCred userCred)
        {
            var token = CustomAuthenticationManager.Authenticate(userCred.Username, userCred.Password);
            if (token == null) { 

                return Unauthorized();  
            }
            return Ok(token);
        }

        //// POST api/<NamesController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<NamesController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<NamesController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
