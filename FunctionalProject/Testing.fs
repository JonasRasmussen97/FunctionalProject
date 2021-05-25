﻿namespace FunctionalProject.Testing

open System
open FsCheck.Experimental
open FunctionalProject.API
open FunctionalProject.Model
open FunctionalProject.Model.Model
open FsCheck
open FunctionalProject.Utilities
open FunctionalProject.DockerReboot
open System.Threading




module Testing =
    
    // API Operations.

    [<StructuredFormatDisplay("")>]
    type apiModel () = 
            let fileVersion = 0
            member __.Get = fileVersion     
        //   member __.getFiles() = getFileListResponse    
        //   member __.fileMetaInformationById(userId: int, id: int) = API.fileMetaInformationById(userId, id)
        //   member __.getDirectoryMetaData(userId: int, dirId: int) = API.getDirectoryMetaData(userId, dirId)
        //   member __.createFile(userId: int, dirId: int, fileName: string, timestamp: string) = API.createFile(userId, dirId, fileName, timestamp)
            
    let spec =
        let getFileMetaInformation id = 
            { new Operation<apiModel,InModel>() with
                member __.Run model = model
                member __.Check (api,model) = 
                    // Bruger den rigtige API metode med pattern matching.
                    let apiResponse = API.getFileMetaData 0 id
                    // Bruger den rigtige metode fra Utilities
                    let modelResponse = Utilities.getModelFileById model.files id
                    match apiResponse.Success, modelResponse.Success, apiResponse.Fail, modelResponse.Fail with 
                        | Some apiFile, Some modelFile, None, None -> (apiFile = modelFile).ToProperty() |@ sprintf "Error: api=%A  Model=%A" apiResponse modelResponse 
                        | None, None, Some apiError, Some modelError -> true.ToProperty() |@ sprintf "Error: api=%A  Model=%A" apiError modelError
                        | Some apiFile, None, None, Some modelError -> false.ToProperty() |@ sprintf "Error: api=%A  Model=%A" apiFile modelError
                        | None, Some modelFile, Some apiError, None -> false.ToProperty() |@ sprintf "Error: api=%A  Model=%A" apiError modelFile
                        | _ -> true.ToProperty() |@ sprintf "Error: api=%A  Model=%A Id=%i" apiResponse modelResponse id
                override __.ToString() = sprintf "getFileMetaInformation fileId=%i" id}
        let getDirectoryMetaInformation id = 
                    {new Operation<apiModel,InModel>() with
                       member __.Run model = model
                       member __.Check (api,model) = 
                           let apiResponse = API.getDirectoryMetaData 0 id
                           let modelResponse = model.directories |> List.find (fun e -> e.id = id)
                           (apiResponse = modelResponse).ToProperty() |@ sprintf "Error: api=%A  Model=%A" apiResponse modelResponse
                       override __.ToString() = sprintf "getDirectoryMetaInformation dirId=%i" id}
        let createFile userId dirId name timeStamp = 
                    {new Operation<apiModel,InModel>() with
                      member __.Run model = 
                          let result = Utilities.createFileModel model dirId userId name timeStamp
                          let apiResult = API.create userId dirId name timeStamp
                          match result.Fail, result.Success with 
                            | None, Some m -> m
                            | Some error, None -> model
                      member __.Check (api,model) =  
                        true.ToProperty()
                       override __.ToString() = sprintf "createFile name=%s" name}
        
        let deleteFile userId fileId fileVersion = 
                    {new Operation<apiModel,InModel>() with
                      member __.Run model = 
                        let result = Utilities.deleteFileModel model userId fileId
                        let apiResult = API.deleteFile userId fileId fileVersion   
                        match result.Fail, result.Success with 
                            | None, Some m -> m
                            | Some error, None -> model
                      member __.Check (api,model) =  
                        true.ToProperty()
                       override __.ToString() = sprintf "deleteFile fileId=%i" fileId}
      
        let create  = 
            { new Setup<apiModel,InModel>() with
                member __.Actual() = 
                  //  let r = Docker.executeShellCommand "docker stop orbit" |> Async.RunSynchronously
                  //  let r = Docker.executeShellCommand "docker run -d --name orbit --rm -p8085:8085 -eCLICOLOR_FORCE=1 cr.orbit.dev/sdu/filesync-server:latest" |> Async.RunSynchronously
                  //  Thread.Sleep 4000
                    new apiModel()

                member __.Model() = Model.model
                
            }
        { new Machine<apiModel,InModel>(10) with
            member __.Setup = create |> Gen.constant |> Arb.fromGen
            member __.Next model =
                let fileIds = Utilities.getAllFileIds model.files
                let directoryIds = Utilities.getAllDirectoryIds model.directories
                let fileIdGenerator = Gen.choose(2, 4)
                let fileVersionGenerator = Gen.oneof [gen {return 1}]
                let userIdGenerator = Gen.oneof [gen { return 0 }]
                let directoryIdGenerator =  Gen.oneof[gen { return 16 }]
                let fileTimeStampGenerator = Gen.oneof[gen { return "123"}]
                let stringGenerator = Gen.oneof[gen {return "a"} ]
                let stringGen = Arb.generate<string>
                let fileIdGen = Gen.frequency [(4,Gen.elements fileIds); (1 ,fileIdGenerator )]
                let fileMetaInformationGen = [Gen.map getFileMetaInformation fileIdGen]
                let directoryMetaInformationGen = [Gen.map getDirectoryMetaInformation directoryIdGenerator] 
                let createFileGen = [Gen.map4 createFile userIdGenerator directoryIdGenerator stringGenerator fileTimeStampGenerator]
                let deleteFileGen = [Gen.map3 deleteFile userIdGenerator fileIdGenerator fileVersionGenerator]
                Gen.oneof (fileMetaInformationGen @ createFileGen @ directoryMetaInformationGen)
        }

    //let config = {Config.Verbose with MaxTest = 1; Replay = Some <| Random.StdGen(1662852042 , 296892251)  }
    let config = {Config.Verbose with MaxTest = 1;  }
    let start = Check.One(config , StateMachine.toProperty spec)

      