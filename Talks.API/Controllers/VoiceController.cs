using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Talks.API;
using Twilio.AspNet.Core;
using Twilio.TwiML;
using Twilio.TwiML.Voice;

namespace Talks.Server.Controllers
{
    [ApiController]
    public class VoiceController : TwilioController
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<VoiceController> logger;
        private readonly TwilioSettings _twilioSettings;

        public VoiceController(IConfiguration configuration, ILogger<VoiceController> logger, IOptions<TwilioSettings> twilioSettings)
        {
            this.configuration = configuration;
            this.logger = logger;
            this._twilioSettings = twilioSettings.Value;
        }

        [HttpPost]
        [Route("voice/outgoing")]
        public TwiMLResult Outgoing([FromForm] string to)
        {
            string twilioPhoneNumber = _twilioSettings.TwilioPhoneNumber;
            var response = new VoiceResponse();
            var dial = new Dial();
            logger.LogInformation($"Calling {to}");
            dial.CallerId = twilioPhoneNumber;
            dial.Number(to);
            response.Append(dial);
            return TwiML(response);
        }

        [HttpPost]
        [Route("voice/incoming")]
        public TwiMLResult Incoming([FromForm] string from)
        {
            logger.LogInformation($"Receiving call from {from}");
            var response = new VoiceResponse();
            var dial = new Dial();
            logger.LogInformation($"Calling blazor_client");
            dial.CallerId = from;
            // client has to match the identity specified in TokenController
            dial.Client("blazor_client");
            response.Append(dial);
            return TwiML(response);
        }
    }
}
