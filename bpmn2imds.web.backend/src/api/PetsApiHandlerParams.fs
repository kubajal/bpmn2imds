namespace bpmn2imds.web.backend

open bpmn2imds.web.backend.Model.Error
open bpmn2imds.web.backend.Model.Pet
open System.Collections.Generic
open System

module PetsApiHandlerParams = 


    
    type CreatePetsStatusCode201Response = {
      content:string;
      
    }
    
    type CreatePetsDefaultStatusCodeResponse = {
      content:Error;
      
    }
    type CreatePetsResult = CreatePetsStatusCode201 of CreatePetsStatusCode201Response|CreatePetsDefaultStatusCode of CreatePetsDefaultStatusCodeResponse


    //#region Query parameters
    [<CLIMutable>]
    type ListPetsQueryParams = {
      limit : int option;
      
    }
    //#endregion

    
    type ListPetsStatusCode200Response = {
      content:Pet[];
      
    }
    
    type ListPetsDefaultStatusCodeResponse = {
      content:Error;
      
    }
    type ListPetsResult = ListPetsStatusCode200 of ListPetsStatusCode200Response|ListPetsDefaultStatusCode of ListPetsDefaultStatusCodeResponse

    type ListPetsArgs = {
      queryParams:Result<ListPetsQueryParams,string>;
    }
    //#region Path parameters
    [<CLIMutable>]
    type ShowPetByIdPathParams = {
      petId : string ;
    }
    //#endregion

    
    type ShowPetByIdStatusCode200Response = {
      content:Pet;
      
    }
    
    type ShowPetByIdDefaultStatusCodeResponse = {
      content:Error;
      
    }
    type ShowPetByIdResult = ShowPetByIdStatusCode200 of ShowPetByIdStatusCode200Response|ShowPetByIdDefaultStatusCode of ShowPetByIdDefaultStatusCodeResponse

    type ShowPetByIdArgs = {
      pathParams:ShowPetByIdPathParams;
    }
    