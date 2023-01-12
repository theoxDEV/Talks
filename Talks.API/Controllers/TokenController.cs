using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Twilio.Jwt.AccessToken;

namespace Talks.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration configuration; 
        private readonly TwilioSettings _twilioSettings;
        public TokenController(IConfiguration configuration, IOptions<TwilioSettings> twilioSettings)
        {
            this.configuration = configuration;
            this._twilioSettings = twilioSettings.Value;
        }

        [HttpGet]
        [EnableCors("BlazorClientPolicy")]
        public string Get()
        {
            string twilioAccountSid = _twilioSettings.TwilioAccountSid;
            string twilioApiKey = _twilioSettings.TwilioApiKey;
            string twilioApiSecret = _twilioSettings.TwilioApiSecret;
            string twiMLApplicationSid = _twilioSettings.TwiMLApplicationSid;

            var grants = new HashSet<IGrant>();
            // Create a Voice grant for this token
            grants.Add(new VoiceGrant
            {
                OutgoingApplicationSid = twiMLApplicationSid,
                IncomingAllow = true
            });

            // Create an Access Token generator
            var token = new Token(
                twilioAccountSid,
                twilioApiKey,
                twilioApiSecret,
                // identity will be used as the client name for incoming dials
                identity: "blazor_client",
                grants: grants
            );

            return token.ToJwt();
        }
    }
}
