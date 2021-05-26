namespace FunctionalProject.Testing

open System
open FsCheck.Experimental
open FunctionalProject.API
open FunctionalProject.Model
open FunctionalProject.Model.Model
open FsCheck
open FunctionalProject.Utilities
open System.Threading




module Testing =
    
    // API Operations.

    [<StructuredFormatDisplay("")>]
    type apiModel () = 
            let fileVersion = 0
            member __.Get = fileVersion     
            
    let spec =
        let getFileMetaInformation id = 
            { new Operation<apiModel,InModel>() with
                member __.Run model = model
                member __.Check (api,model) = 
                    let apiResponse = API.getFileMetaData 0 id
                    let modelResponse = Utilities.getModelFileById model.files id
                    match apiResponse.Pass, modelResponse.Pass, apiResponse.Fail, modelResponse.Fail with 
                        | Some apiFile, Some modelFile, None, None -> (apiFile = modelFile).ToProperty() |@ sprintf "Error: api=%A  Model=%A" apiResponse modelResponse 
                        | None, None, Some apiError, Some modelError -> (apiError = modelError) |@ sprintf "Error: api=%A  Model=%A" apiError modelError
                        | Some apiFile, None, None, Some modelError -> false.ToProperty() |@ sprintf "Error: api=%A  Model=%A" apiFile modelError
                        | None, Some modelFile, Some apiError, None -> false.ToProperty() |@ sprintf "Error: api=%A  Model=%A" apiError modelFile
                        | _ -> true.ToProperty() |@ sprintf "Error: api=%A  Model=%A Id=%i" apiResponse modelResponse id
                override __.ToString() = sprintf "getFileMetaInformation fileId=%i" id}
        let getDirectoryMetaInformation id = 
            {new Operation<apiModel,InModel>() with
                member __.Run model = model
                member __.Check (api,model) = 
                    let apiResponse = API.getDirectoryMetaData 0 id
                    let modelResponse = Utilities.getModelDirectoryById model.directories id
                    match apiResponse.Pass, modelResponse.Pass, apiResponse.Fail, modelResponse.Fail with 
                        | Some apiDirectory, Some modelDirectory, None, None -> (apiDirectory = modelDirectory).ToProperty() |@ sprintf "Error: api=%A  Model=%A" apiResponse modelResponse 
                        | None, None, Some apiError, Some modelError -> (apiError = modelError) |@ sprintf "Error: api=%A  Model=%A" apiError modelError
                        | Some apiDirectory, None, None, Some modelError -> false.ToProperty() |@ sprintf "Error: api=%A  Model=%A" apiDirectory modelError
                        | None, Some modelDirectory, Some apiError, None -> false.ToProperty() |@ sprintf "Error: api=%A  Model=%A" apiError modelDirectory
                        | _ -> true.ToProperty() |@ sprintf "Error: api=%A  Model=%A Id=%i" apiResponse modelResponse id
                override __.ToString() = sprintf "getDirectoryMetaInformation dirId=%i" id}
        let createFile userId dirId name timeStamp = 
                    {new Operation<apiModel,InModel>() with
                      member __.Run model = 
                          let modelResponse = Utilities.createFileModel model dirId userId name timeStamp
                          let apiResponse = API.createFile userId dirId name timeStamp
                          match modelResponse.Pass,modelResponse.Fail with  
                            | Some m, None -> m
                            | None, Some error -> model
                      member __.Check (api,model) =  
                        true.ToProperty()
                       override __.ToString() = sprintf "createFile name=%s" name}
        
        let deleteFile userId fileId fileVersion = 
                    {new Operation<apiModel,InModel>() with
                      member __.Run model = 
                        let modelResponse = Utilities.deleteFileModel model userId fileId  
                        let apiResponse = API.deleteFile userId fileId fileVersion  
                        match modelResponse.Pass, modelResponse.Fail with 
                           | Some newModel, None -> newModel
                           | None, Some error -> model
                      member __.Check (api,model) =  
                        true.ToProperty()
                       override __.ToString() = sprintf "deleteFile fileId=%i" fileId}
      
        let create  = 
            { new Setup<apiModel,InModel>() with
                member __.Actual() = new apiModel()
                member __.Model() = Model.model 
            }
        { new Machine<apiModel,InModel>(5) with
            member __.Setup = create |> Gen.constant |> Arb.fromGen
            member __.Next model =
                let fileIds = Utilities.getAllFileIds model.files
                let directoryIds = Utilities.getAllDirectoryIds model.directories
                let userIdGen = Gen.oneof [gen { return 0 }]
                let fileVersionGen = Gen.oneof [gen {return 1}]
                let randomIdGen = Gen.choose(0, 100);
                let fileIdGen = Gen.frequency [(4,Gen.elements fileIds)]
                let directoryIdGen =  Gen.elements directoryIds
                let fileTimeStampGen = Gen.oneof[gen { return "123"}]
                let filenameGen = Gen.oneof[gen { return "abe"}]
                
                let fileMetaInformationGen = [Gen.map getFileMetaInformation fileIdGen]
                let directoryMetaInformationGen = [Gen.map getDirectoryMetaInformation directoryIdGen] 
                let createFileGen = [Gen.map4 createFile userIdGen directoryIdGen filenameGen fileTimeStampGen]
                let deleteFileGen = [Gen.map3 deleteFile userIdGen fileIdGen fileVersionGen]
                Gen.oneof (fileMetaInformationGen @ directoryMetaInformationGen @ createFileGen @ deleteFileGen)
        }

    //let config = {Config.Verbose with MaxTest = 1; Replay = Some <| Random.StdGen(1662852042 , 296892251)  }
    let config = {Config.Verbose with MaxTest = 1;  }
    let start = Check.One(config , StateMachine.toProperty spec)

      