namespace bpmn2imds.web.backend.Model

open System
open System.Collections.Generic
open Newtonsoft.Json

module Pet = 

  //#region Pet

  [<CLIMutable>]
  type Pet = {
    [<JsonProperty(PropertyName = "id")>]
    Id : int64;
    [<JsonProperty(PropertyName = "name")>]
    Name : string;
    [<JsonProperty(PropertyName = "tag")>]
    Tag : string;
  }
  
  //#endregion
  