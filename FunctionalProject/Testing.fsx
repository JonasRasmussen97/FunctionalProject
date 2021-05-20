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
        let userIdGenerator = Gen.oneof [ gen { return 0 }]
        let directoryIdGenerator = Gen.choose(1, 21)
        // Generates a random string.
        let fileTimeStampGenerator = Gen.oneof[gen { return "123"}]
        let stringGenerator = Gen.oneof[gen {return "Hello.txt"}; gen {return "ThisWorksToo.txt"}; gen {return "AnotherOne.txt"}] 
        let startingFileId = 5
        // API Operations.
        type API() =
            member __.getFiles() = API.getFileListResponse().statusCode   
            member __.fileMetaInformationById(userId: int, id: int) = API.fileMetaInformationById(userId, id)
            member __.getDirectoryMetaData(userId: int, dirId: int) = API.getDirectoryMetaData(userId, dirId)
            member __.createFile(userId: int, dirId: int, fileName: string, timestamp: string) = API.createFile(userId, dirId, fileName, timestamp)
            

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
             let getDirectoryMetaInformation id = { new Command<API, string>() with
                        // Here we retrieve the id of the API file.
                override __.RunActual api = api.getDirectoryMetaData(0, id).name; api
                        // Here we retrieve the id of the Model file.
                override __.RunModel directoryName = (Model.directories.Head.Item (id)).name
                        // Here we match the two id's to see if the states of both the api and the model are equal.
                override __.Post(api, directoryName ) = api.getDirectoryMetaData(0, id).name = directoryName  |@ sprintf "model: %s <> api: %s" directoryName (api.getDirectoryMetaData(0,id).name)
                        // Print actual and model.
                override __.ToString() = "getDirectoryMetaInformation"
            }
                // Example of creation > files.Head.Add(5, {id=5; version=1; versionChanged=1; name="Test.txt"; parentId=15; timestamp="123"});;
             let createFile userId dirId fileName timestamp = { new Command<API, string>() with
                        // Here we retrieve the id of the API file.
                override __.RunActual api = api.createFile(userId, dirId, fileName, timestamp).name; api
                        // Here we retrieve the id of the Model file.
                        // (Model.files.Head.Add(startingFileId, {id=startingFileId; version=1; versionChanged=1; name=fileName; parentId=dirId; timestamp=timestamp})
                override __.RunModel newFileName = ((Model.files.Head.Add(startingFileId, {id=startingFileId; version=1; versionChanged=1; name=fileName; parentId=dirId; timestamp=timestamp})).Item startingFileId).name
                        // Here we match the two id's to see if the states of both the api and the model are equal.
                override __.Post(api, newFileName ) = api.fileMetaInformationById(0, startingFileId).name = (newFileName) |@ sprintf "model: %s <> api: %s" newFileName (api.fileMetaInformationById(0, startingFileId).name)
                        // Print actual and model.
                override __.ToString() = "createFile"
            }     
            { new ICommandGenerator<API,string> with
                member __.InitialActual = API()
                member __.InitialModel = ""
                member __.Next model = 
                        let fileMetaInformationGen = [Gen.map getFileMetaInformation fileIdGenerator]
                        let directoryMetaInformationGen = [Gen.map getDirectoryMetaInformation directoryIdGenerator] 
                        let createFileGen = [Gen.map4 createFile userIdGenerator directoryIdGenerator stringGenerator fileTimeStampGenerator]
                        Gen.oneof (fileMetaInformationGen @ directoryMetaInformationGen @ createFileGen)
            }
                
        //Check.One({Config.Verbose with MaxTest = 1;}, Command.toProperty spec);;
        Check.Quick(Command.toProperty spec)

      