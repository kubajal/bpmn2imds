namespace bpmn2imds.web.backend

open PetsApiHandlerParams
open PetsApiServiceImplementation
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Http
open Newtonsoft.Json
open Microsoft.Azure.WebJobs
open System.IO

module PetsApiHandlers =

    /// <summary>
    /// 
    /// </summary>

    //#region CreatePets
    /// <summary>
    /// Create a pet
    /// </summary>
   [<FunctionName("CreatePets")>]
    let CreatePets
        ([<HttpTrigger(Extensions.Http.AuthorizationLevel.Anonymous, "POST", Route = "/v1/pets")>]
        req:HttpRequest ) =
      
      let result = PetsApiService.CreatePets ()
      match result with 
      | CreatePetsStatusCode201 resolved ->
          let content = resolved.content
          let responseContentType = "text/plain"
          ContentResult(Content = content, ContentType = responseContentType, StatusCode = System.Nullable(201)) 
      | CreatePetsDefaultStatusCode resolved ->
          let content = JsonConvert.SerializeObject resolved.content
          let responseContentType = "application/json"
          ContentResult(Content = content, ContentType = responseContentType, StatusCode = System.Nullable(0)) 

    //#region ListPets
    /// <summary>
    /// List all pets
    /// </summary>
   [<FunctionName("ListPets")>]
    let ListPets
        ([<HttpTrigger(Extensions.Http.AuthorizationLevel.Anonymous, "GET", Route = "/v1/pets")>]
        req:HttpRequest ) =
      
      let result = PetsApiService.ListPets ()
      match result with 
      | ListPetsStatusCode200 resolved ->
          let content = JsonConvert.SerializeObject resolved.content
          let responseContentType = "application/json"
          ContentResult(Content = content, ContentType = responseContentType, StatusCode = System.Nullable(200)) 
      | ListPetsDefaultStatusCode resolved ->
          let content = JsonConvert.SerializeObject resolved.content
          let responseContentType = "application/json"
          ContentResult(Content = content, ContentType = responseContentType, StatusCode = System.Nullable(0)) 

    //#region ShowPetById
    /// <summary>
    /// Info for a specific pet
    /// </summary>
   [<FunctionName("ShowPetById")>]
    let ShowPetById
        ([<HttpTrigger(Extensions.Http.AuthorizationLevel.Anonymous, "GET", Route = "/v1/pets/{petId}")>]
        req:HttpRequest ) =
      
      let result = PetsApiService.ShowPetById ()
      match result with 
      | ShowPetByIdStatusCode200 resolved ->
          let content = JsonConvert.SerializeObject resolved.content
          let responseContentType = "application/json"
          ContentResult(Content = content, ContentType = responseContentType, StatusCode = System.Nullable(200)) 
      | ShowPetByIdDefaultStatusCode resolved ->
          let content = JsonConvert.SerializeObject resolved.content
          let responseContentType = "application/json"
          ContentResult(Content = content, ContentType = responseContentType, StatusCode = System.Nullable(0)) 


      

