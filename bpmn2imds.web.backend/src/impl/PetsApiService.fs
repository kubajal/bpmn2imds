namespace bpmn2imds.web.backend
open bpmn2imds.web.backend.Model.Error
open bpmn2imds.web.backend.Model.Pet
open PetsApiHandlerParams
open PetsApiServiceInterface
open System.Collections.Generic
open System

module PetsApiServiceImplementation =
    
    //#region Service implementation
    type PetsApiServiceImpl() = 
      interface IPetsApiService with
      
        member this.CreatePets () =
          if true then 
            let content = "Null response" 
            CreatePetsStatusCode201 { content = content }
          else
            let content = "unexpected error" :> obj :?> Error // this cast is obviously wrong, and is only intended to allow generated project to compile   
            CreatePetsDefaultStatusCode { content = content }

        member this.ListPets () =
          if true then 
            let content = "A paged array of pets" :> obj :?> Pet[] // this cast is obviously wrong, and is only intended to allow generated project to compile   
            ListPetsStatusCode200 { content = content }
          else
            let content = "unexpected error" :> obj :?> Error // this cast is obviously wrong, and is only intended to allow generated project to compile   
            ListPetsDefaultStatusCode { content = content }

        member this.ShowPetById () =
          if true then 
            let content = "Expected response to a valid request" :> obj :?> Pet // this cast is obviously wrong, and is only intended to allow generated project to compile   
            ShowPetByIdStatusCode200 { content = content }
          else
            let content = "unexpected error" :> obj :?> Error // this cast is obviously wrong, and is only intended to allow generated project to compile   
            ShowPetByIdDefaultStatusCode { content = content }

      //#endregion

    let PetsApiService = PetsApiServiceImpl() :> IPetsApiService