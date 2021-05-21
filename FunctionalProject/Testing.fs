namespace FunctionalProject.Testing

open FSharp
open FsCheck.Experimental
open FunctionalProject.API
open FunctionalProject.Model
open FunctionalProject.Model.Model
open FsCheck
open FunctionalProject.Utilities
open FunctionalProject.DockerReboot
open System.Threading




module Testing =
    // Generates a random file id to be used. Needs to be added to the model & the api.
    let fileIdGenerator = Gen.choose(2, 4)
    let userIdGenerator = Gen.oneof [ gen { return 0 }]
    let directoryIdGenerator = Gen.choose(1, 21)
    // Generates a random string.
    let fileTimeStampGenerator = Gen.oneof[gen { return "123"}]
    let stringGenerator = Gen.oneof[gen {return "Hello.txt"}; gen {return "ThisWorksToo.txt"}; gen {return "AnotherOne.txt"}] 
    let startingFileId = 5
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
                    let apiResponse = API.fileMetaInformationById(0, id)
                    let modelResponse = model.files |> List.find (fun e -> e.id = id)
                    (apiResponse = modelResponse).ToProperty() |@ sprintf "Error: api=%A  Model=%A" apiResponse modelResponse
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
                          match result.Fail, result.Success with 
                            | None, Some m -> m
                            | Some error, None -> model
                      member __.Check (api,model) = 
                          let apiResponse = API.getFileMetaData userId model.currentFileId
                          match apiResponse.Fail, apiResponse.Success with 
                            | None, Some m -> m
                            | Some error, None -> model
                          let modelResponse = model.files.Item model.currentFileId
                          (apiResponse = modelResponse).ToProperty() |@ sprintf "Error: api=%A  Model=%A" apiResponse modelResponse
                       override __.ToString() = sprintf "createFile fileId=%i" model.currentFileId}
                        
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
                let fileIdGen = Gen.frequency [(2,Gen.elements fileIds); (1 ,fileIdGenerator )]
                let fileMetaInformationGen = [Gen.map getFileMetaInformation fileIdGenerator]
                let directoryMetaInformationGen = [Gen.map getDirectoryMetaInformation directoryIdGenerator] 
                let createFileGen = [Gen.map4 createFile userIdGenerator directoryIdGenerator stringGenerator fileTimeStampGenerator]
                Gen.oneof (fileMetaInformationGen @ directoryMetaInformationGen @ createFileGen)
        }

    //let config = {Config.Verbose with MaxTest = 1; Replay = Some <| Random.StdGen(1662852042 , 296892251)  }
    let config = {Config.Verbose with MaxTest = 1;  }
    let start = Check.One(config , StateMachine.toProperty spec)

      