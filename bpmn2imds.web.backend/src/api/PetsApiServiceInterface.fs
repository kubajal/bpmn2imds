namespace bpmn2imds.web.backend
open PetsApiHandlerParams
open System
open Microsoft.AspNetCore.Http


module PetsApiServiceInterface =
    
    //#region Service interface
    type IPetsApiService = 
      abstract member CreatePets : unit -> CreatePetsResult
      abstract member ListPets : unit -> ListPetsResult
      abstract member ShowPetById : unit -> ShowPetByIdResult
    //#endregion