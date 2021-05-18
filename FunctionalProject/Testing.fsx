namespace Project

    #r "nuget: FsCheck";;
    #r "nuget: FSharp.Data";;
    open FSharp.Data;;
    open FsCheck;;
    #load "Model.fsx";;
    #load "API.fsx";;

    module Testing =
        // Generates a random file id to be used. Needs to be added to the model & the api.
        let fileIdGenerator = Gen.choose(2, 4)
        let directoryIdGenerator() = Gen.choose(1, 21) |> Gen.sample 0 1 |> Seq.exactlyOne
        // Customer generator that creates files.
        let fileGenerator() = Arb.generate<Model.File>|> Gen.nonEmptyListOf |> Gen.sample 0 1 |> Seq.exactlyOne
        // Generates a random string.
        let stringGenerator() = Arb.generate<NonEmptyString>|> Gen.sample 100 1
        
        // API Operations.
        type API() =
            member __.getFiles() = API.getFileListResponse().statusCode   
            member __.fileMetaInformationById(userId: int, id: int) = API.fileMetaInformationById(userId, id)
            member __.getDirectoryMetaData(userId: int, dirId: int) = API.getDirectoryMetaData(userId, dirId)
            
        // Model Based Testing 
        let spec =
        // First command here.Test for getting a file and matching the id up against the model & the api.
            let getFileMetaInformation id = { new Command<API, string>() with
                        // Here we retrieve the id of the API file.
                override __.RunActual api = api.fileMetaInformationById(0, id); api
                        // Here we retrieve the id of the Model file.
                override __.RunModel fileName = (Model.files.Head.Item (id)).name
                        // Here we match the two id's to see if the states of both the api and the model are equal.
                override __.Post(api, fileName ) = api.fileMetaInformationById(0, id).name = fileName  |@ sprintf "model: %s <> api: %s" fileName (api.fileMetaInformationById(0, id).name)
                        // Print actual and model.
                override __.ToString() = "getFileMetaInformation"
            }  
        // Check directory meta information.
             let getDirectoryMetaInformation = { new Command<API, string>() with
                        // Here we retrieve the id of the API file.
                override __.RunActual api = api.getDirectoryMetaData(0, directoryIdGenerator()).name; api
                        // Here we retrieve the id of the Model file.
                override __.RunModel directoryName = (Model.directories.Head.Item (directoryIdGenerator())).name
                        // Here we match the two id's to see if the states of both the api and the model are equal.
                override __.Post(api, directoryName ) = api.getDirectoryMetaData(0, directoryIdGenerator()).name = directoryName  |@ sprintf "model: %s <> api: %s" directoryName (api.getDirectoryMetaData(0,directoryIdGenerator()).name)
                        // Print actual and model.
                override __.ToString() = "getDirectoryMetaInformation"
            }    
            { new ICommandGenerator<API,string> with
                member __.InitialActual = API()
                member __.InitialModel = ""
                member __.Next model = 
                        let fileMetaInformationGen = [Gen.map getFileMetaInformation fileIdGenerator] 
                        Gen.oneof (fileMetaInformationGen)
            }
                
        //Check.One({Config.Verbose with MaxTest = 1;}, Command.toProperty spec);;
        Check.Quick(Command.toProperty spec)

      