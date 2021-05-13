namespace Project

    #r "nuget: FsCheck";;
    #r "nuget: FSharp.Data";;
    open FSharp.Data;;
    open FsCheck;;
    #load "Model.fsx";;
    #load "API.fsx";;

    module Testing =
        // Generates a random file id to be used. Needs to be added to the model & the api.
        let fileIdGenerator() = Gen.choose(2, 4) |> Gen.sample 0 1 |> Seq.exactlyOne
        // Customer generator that creates files.
        let fileGenerator() = Arb.generate<Model.File>|> Gen.nonEmptyListOf |> Gen.sample 0 1 |> Seq.exactlyOne
        // Generates a random string.
        let stringGenerator() = Arb.generate<NonEmptyString>|> Gen.sample 100 1
        
        
        // API Operations.
        type API() =
            member __.getFiles() = API.getFileListResponse().statusCode   
            member __.getSpecificFile() = API.getFileById(fileIdGenerator())

        // Model Based Testing 

        let spec =
        // First command here.Test for getting a file and matching the id up against the model & the api.
            let getFile = { new Command<API, int>() with
                        // Here we retrieve the id of the API file.
                override __.RunActual api = api.getSpecificFile().id; api
                        // Here we retrieve the id of the Model file.
                override __.RunModel fileId = (Model.files.Head.Item (fileIdGenerator())).id
                        // Here we match the two id's to see if the states of both the api and the model are equal.
                override __.Post(api, fileId ) = api.getSpecificFile().id = fileId  |@ sprintf "model: %i <> %A" fileId api
                        // Print actual and model.
                override __.ToString() = ("Operation: GetFile" + " " + "Value: " + string (fileIdGenerator()))
            }                      
            let createFile = { new Command<API, int>() with 
                override __.RunActual api = api.getSpecificFile().id; api
                override __.RunModel fileId = (Model.files.Head.Item (fileIdGenerator())).id
            }
            { new ICommandGenerator<API,int> with
                member __.InitialActual = API()
                member __.InitialModel = 0
                member __.Next model = Gen.elements [getFile]}

      
        Check.One({Config.Verbose with MaxTest = 2;}, Command.toProperty spec);;


      