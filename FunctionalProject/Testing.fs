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
                    let modelResponse = model.FileMetaData |> List.find (fun e -> e.id = id)
                    (apiResponse = modelResponse).ToProperty() |@ sprintf ""
                override __.ToString() = "getFileMetaInformation"}

        let getDirectoryMetaInformation id = 
                   { new Operation<apiModel,InModel>() with
                       member __.Run model = model
                       member __.Check (api,model) = 
                           let apiResponse = API.getDirectoryMetaData(0, id)
                           let modelResponse = model.directories |> List.find (fun e -> e.id = id)
                           (apiResponse = modelResponse).ToProperty() |@ sprintf "Error api=%A  Model=%A" apiResponse modelResponse
                       override __.ToString() = sprintf "getDirectoryMetaInformation dirId=%i" id }
        let createFile userId dirId name timeStamp = 
                          { new Operation<apiModel,InModel>() with
                              member __.Run model = 
                                  let result = Utilities.createFileModel model userId 0
                                  match result.Fail, result.Success with 
                                    | None, Some m -> m
                                    | Some error, None -> model
                              member __.Check (api,model) = 
                                  true.ToProperty()
                              override __.ToString() = sprintf "createFile fileId=%i" dirId}

        let create  = 
            { new Setup<apiModel,InModel>() with
                member __.Actual() = 
                  //  let r = Docker.executeShellCommand "docker stop orbit" |> Async.RunSynchronously
                  //  let r = Docker.executeShellCommand "docker run -d --name orbit --rm -p8085:8085 -eCLICOLOR_FORCE=1 cr.orbit.dev/sdu/filesync-server:latest" |> Async.RunSynchronously
                  //  Thread.Sleep 4000
                    new apiModel()

                member __.Model() = 
                    let model = {
                            FileMetaData = [
                            {id=2; version=1; versionChanged=1; name="README.txt"; parentId=15; timestamp="637479675580000000"}
                            {id=3; version=1; versionChanged=1; name="README.txt"; parentId=16; timestamp="637479675580000000"}
                            {id=4; version=1; versionChanged=1; name="INTRO.txt"; parentId=9; timestamp="637479675580000000"}
                            ]
                            directories = [ 
                            {id=1; name="server-files"; path="server-files/"; version=1; parent=Some({id=(0)}); is_checked_out=false; is_default=false}
                            {id=2; name="Projects"; path="server-files/Projects/"; version=1; parent=Some({id=(1)}); is_checked_out=false; is_default=false}
                            {id=3; name="Project deliverables"; path="server-files/Project deliverables/"; version=1; parent=Some({id=(1)}); is_checked_out=false; is_default=false}
                            {id=4; name="Project Templates"; path="server-files/Project Templates/"; version=1; parent=Some({id=(1)}); is_checked_out=false; is_default=false}
                            {id=5; name="Standard - 1"; path="server-files/Project Templates/Standard - 1/"; version=1; parent=Some({id=(4)}); is_checked_out=false; is_default=false}
                            {id=6; name="deliverables"; path="server-files/Project Templates/Standard - 1/deliverables/"; version=1; parent=Some({id=(5)}); is_checked_out=false; is_default=false}
                            {id=7; name="explorer_root"; path="server-files/Project Templates/Standard - 1/explorer_root/"; version=1; parent=Some({id=(5)}); is_checked_out=false; is_default=false}
                            {id=8; name="Project emails"; path="server-files/Project emails/"; version=1; parent=Some({id=(1)}); is_checked_out=false; is_default=false}
                            {id=9; name="Shared files"; path="server-files/Shared files/"; version=1; parent=Some({id=(1)}); is_checked_out=false; is_default=false}
                            {id=10; name="Companies"; path="server-files/Companies/"; version=1; parent=Some({id=(1)}); is_checked_out=false; is_default=false}
                            {id=11; name="snapshots"; path="server-files/snapshots/"; version=1; parent=Some({id=(1)}); is_checked_out=false; is_default=false}
                            {id=12; name="Sales Activities"; path="server-files/Sales Activities/"; version=1; parent=Some({id=(1)}); is_checked_out=false; is_default=false}
                            {id=13; name="File importer"; path="server-files/File importer/"; version=1; parent=Some({id=(1)}); is_checked_out=false; is_default=false}
                            {id=14; name="Users"; path="server-files/Users/"; version=1; parent=Some({id=(1)}); is_checked_out=false; is_default=false}
                            {id=15; name="rw"; path="server-files/Users/rw/"; version=1; parent=Some({id=(14)}); is_checked_out=false; is_default=false}
                            {id=16; name="ro"; path="server-files/Users/ro/"; version=1; parent=Some({id=(14)}); is_checked_out=false; is_default=false}
                            {id=17; name="Project 1"; path="server-files/Projects/Project 1/"; version=1; parent=Some({id=(2)}); is_checked_out=false; is_default=false}
                            {id=18; name="Project 2"; path="server-files/Projects/Project 2/"; version=1; parent=Some({id=(2)}); is_checked_out=false; is_default=false}
                            {id=19; name="none"; path="server-files/Users/none/"; version=1; parent=Some({id=(14)}); is_checked_out=false; is_default=false}
                            {id=20; name="Delete me"; path="server-files/Shared files/Delete me/"; version=1; parent=Some({id=(9)}); is_checked_out=false; is_default=false}
                            {id=21; name="Delete me too"; path="server-files/File importer/Delete me too/"; version=1; parent=Some({id=(13)}); is_checked_out=false; is_default=false}
                            ]
                            currentFileId = 5
                            }
                    model
                
                }
        { new Machine<apiModel,InModel>(10) with
            member __.Setup = create |> Gen.constant |> Arb.fromGen
            member __.Next model =
                let fileIds = Utilities.getAllFileIds model.FileMetaData
                let fileIdGen = Gen.frequency [(2,Gen.elements fileIds); (1 ,fileIdGenerator )]
                let fileMetaInformationGen = [Gen.map getFileMetaInformation fileIdGenerator]
                let directoryMetaInformationGen = [Gen.map getDirectoryMetaInformation directoryIdGenerator] 
                let createFileGen = [Gen.map4 createFile userIdGenerator directoryIdGenerator stringGenerator fileTimeStampGenerator]
                Gen.oneof (fileMetaInformationGen @ directoryMetaInformationGen @ createFileGen)
                }
   
        
    (*
    // Model Based Testing 
    let spec =
    // First command here.Test for getting a file and matching the id up against the model & the api.
  
       
            // Example of creation > files.Head.Add(5, {id=5; version=1; versionChanged=1; name="Test.txt"; parentId=15; timestamp="123"});;
      let createFile id = { new Command<apiModel, Model.InModel>() with
                         // Here we retrieve the id of the API file.
                 override __.RunActual api =  api
                         // Here we retrieve the id of the Model file.
                 override __.RunModel model = model
                         // Here we match the two id's to see if the states of both the api and the model are equal.
                 override __.Post(api, model ) = 
                         let apiResponse = API.fileMetaInformationById(0, id)
                         let modelResponse = model.FileMetaData |> List.find (fun e -> e.id = id)
                         (apiResponse = modelResponse).ToProperty()  
                         
                         //fileMetaInformationById(0, id).name = fileName  |@ sprintf "model: %s <> api: %s" fileName (api.fileMetaInformationById(0, id).name)
                         // Print actual and model.
                 override __.ToString() = "getFileMetaInformation" }  

        { new ICommandGenerator<apiModel, InModel> with
            member __.InitialActual = apiModel()
            member __.InitialModel = 
                    let model = {
                            FileMetaData = [
                            {id=2; version=1; versionChanged=1; name="README.txt"; parentId=15; timestamp="637479675580000000"}
                            {id=3; version=1; versionChanged=1; name="README.txt"; parentId=16; timestamp="637479675580000000"}
                            {id=4; version=1; versionChanged=1; name="INTRO.txt"; parentId=9; timestamp="637479675580000000"}
                            ]
                            }
                    model
            member __.Next model = 
                    let fileMetaInformationGen = [Gen.map getFileMetaInformation fileIdGenerator]
                    //let directoryMetaInformationGen = [Gen.map getDirectoryMetaInformation directoryIdGenerator] 
                    //let createFileGen = [Gen.map4 createFile userIdGenerator directoryIdGenerator stringGenerator fileTimeStampGenerator]
                    Gen.oneof (fileMetaInformationGen)
        }
       *)         
    //Check.One({Config.Verbose with MaxTest = 1;}, Command.toProperty spec);;

    //let config = {Config.Verbose with MaxTest = 1; Replay = Some <| Random.StdGen(1662852042 , 296892251)  }
    let config = {Config.Verbose with MaxTest = 1;  }
    let start = Check.One(config , StateMachine.toProperty spec)

      