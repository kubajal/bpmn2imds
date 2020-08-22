
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace bpmn2imds.web.api
{
  public class IVerificationControllerImplementation : IVerificationController
  {

    Task<ActionResult<ICollection<Deadlock>>> IVerificationController.ValidateAsync(int queuedAgents, RequestPayload payload)
    {
        var t = new Task<ActionResult<ICollection<Deadlock>>>(() => new ObjectResult(new List<Deadlock>() { new Deadlock {Id = "asdf", BlockedElement=null, Path = null} }));
        t.Start();
        return t;
    }
  }
}