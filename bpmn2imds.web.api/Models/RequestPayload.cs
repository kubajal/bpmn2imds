using System.ComponentModel.DataAnnotations;

namespace bpmn2imds.web.api.Controllers
{
    public class RequestPayload
    {
        [Required]
        public string xml { get; set; }

        [Required]
        public int queuedAgents { get; set; }

        public RequestPayload(string xml)
        {
            this.xml = xml;
        }
        public RequestPayload() { }
    }
}
