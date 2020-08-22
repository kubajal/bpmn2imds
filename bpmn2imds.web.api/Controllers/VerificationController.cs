using bpmn2imds;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace bpmn2imds.web.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VerificationController : ControllerBase
    {
        [HttpPost]
        [Route("validate")]
        // to do: replace List<Object> with List<Deadlock>
        public IActionResult Validate([FromBody] RequestPayload Payload)
        {
            int queuedAgents = Payload.queuedAgents;
            if (queuedAgents > 3)
                return new ObjectResult("Cannot queue more than 3 agents on AND splits") 
                {
                    StatusCode = 400
                };
            if (queuedAgents <= 0)
                return new ObjectResult("Queued agents count must be positive") 
                {
                    StatusCode = 400
                };
            if (Payload == null)
            {
                return new ObjectResult("Payload was null") 
                {
                    StatusCode = 400
                };
            }
            string XML = Payload.xml;
            if (XML == null)
            {
                return new ObjectResult("XML was null") 
                {
                    StatusCode = 400
                };
            }

            // to do: implement diagram validation
            
            return new ObjectResult("Processed...") 
            {
                StatusCode = 200
            };
        }
    }
}

