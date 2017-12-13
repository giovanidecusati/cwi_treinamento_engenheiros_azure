using FluentValidator;
using Meetup.WebApi.Commands;
using Meetup.WebApi.Infrastructure.Data;
using Meetup.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Meetup.WebApi.Infrastructure.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Meetup.WebApi.Infrastructure.IntegrationEvents;

namespace Meetup.WebApi.Controllers
{
    [AllowAnonymous]
    [Route("v1/Account")]
    public class AccountControllerV1 : ControllerBase
    {
        private readonly IEventBus _eventBus;
        private readonly MeetupDbcontext _meetupDbcontext;
        private readonly TokenOptions _tokenOptions;
        private readonly JsonSerializerSettings _serializerSettings;
        private User _user;

        public AccountControllerV1(MeetupDbcontext meetupDbcontext, IOptions<TokenOptions> jwtOptions, IEventBus eventBus)
        {
            _eventBus = eventBus;
            _meetupDbcontext = meetupDbcontext;
            _tokenOptions = jwtOptions.Value;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }


        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegisterUserCommand command)
        {
            var userCreatedEvent = new UserCreatedIntegrationEvent(command.Name, command.Email, command.Password);

            await _eventBus.PublishAsync(userCreatedEvent);

            return Ok(new { EventId = userCreatedEvent.Id });
        }

        [Route("authenticate")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginCommand command)
        {
            if (command == null)
                return BadRequest();

            var identity = await GetClaims(command);
            if (identity == null)
                return BadRequest(new { Messages = new List<Notification> { new Notification("User", "User not found.") } });

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, command.Email),
                new Claim(JwtRegisteredClaimNames.NameId, command.Email),
                new Claim(JwtRegisteredClaimNames.Email, command.Email),
                new Claim(JwtRegisteredClaimNames.Sub, command.Email),
                new Claim(JwtRegisteredClaimNames.Jti, await _tokenOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_tokenOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                identity.FindFirst("Meetup")
            };

            var jwt = new JwtSecurityToken(
                issuer: _tokenOptions.Issuer,
                audience: _tokenOptions.Audience,
                claims: claims.AsEnumerable(),
                notBefore: _tokenOptions.NotBefore,
                expires: _tokenOptions.Expiration,
                signingCredentials: _tokenOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                token = encodedJwt,
                expires = (int)_tokenOptions.ValidFor.TotalSeconds,
                user = new
                {
                    id = _user.Id,
                    name = _user.Name,
                    email = _user.Email,
                }
            };

            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            return new OkObjectResult(json);
        }

        private static void ThrowIfInvalidOptions(TokenOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
                throw new ArgumentException(nameof(TokenOptions.ValidFor));

            if (options.SigningCredentials == null)
                throw new ArgumentNullException(nameof(TokenOptions.SigningCredentials));

            if (options.JtiGenerator == null)
                throw new ArgumentNullException(nameof(TokenOptions.JtiGenerator));
        }

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        private Task<ClaimsIdentity> GetClaims(LoginCommand command)
        {
            var user = _meetupDbcontext.Users.Where(z => z.Email == command.Email).FirstOrDefault();

            if (user == null)
                return Task.FromResult<ClaimsIdentity>(null);

            if (!user.CheckPassword(command.Password))
                return Task.FromResult<ClaimsIdentity>(null);

            _user = user;

            return
                Task.FromResult(
                    new ClaimsIdentity(
                        new GenericIdentity(user.Email, "Token"),
                        new[] { new Claim("Meetup", "User") }));
        }
    }
}
