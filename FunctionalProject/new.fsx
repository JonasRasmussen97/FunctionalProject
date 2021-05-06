#r "nuget: Http.fs"
open HttpFs.Client
open Hopac
#r "nuget: FsCheck"
open FsCheck
open System.Text.Json
#r "nuget: FSharp.Data"
open FSharp.Data

type File = 
    struct
        val id: int
        val name: string
        val size: string
        val mimetype: string
        val parent_id: int
        val version: int
        val created_at: string
        val modified_at: string
        val ms_timestamp: string
        val path: string
        val snapshots_enabled: bool

        // Defines the constructor.
        new (id, name, size, mimetype, parent_id, version, created_at, modified_at, ms_timestamp, path, snapshots_enabled) = {id = id; name = name; size = size; mimetype = mimetype; parent_id = parent_id; version = version; created_at = created_at;
         modified_at = modified_at; ms_timestamp = ms_timestamp; path = path; snapshots_enabled = snapshots_enabled;}
    end
// Defines a Directory type as it is according to the Orbit API.
type Orbit_Directory = 
    struct 
        val id: int
        val name: string
        val path: string
        val version: int
        val parent_id: int
        val is_checked_out: bool
        val is_default: bool
    
        // Defines the constructor.
        new (id, name, path, version, parent_id, is_checked_out, is_default) = {id = id; name = name; path=path; parent_id=parent_id; version=version; is_checked_out = is_checked_out; is_default = is_default}
    end
// Defines a user type as it is according to the Orbit API.
type User = 
    struct
        val id: int
        val name: string
        val initials: string
        
        // Defines the constructor.
        new (id, name, initials) = {id = id; name = name; initials = initials;}
    end;;

let files = [Map.empty.
Add(2, new File(2, "README.txt", "78", "text/plain", 15, 1, "2021-02-19T15:20:35.702Z", "2021-02-19T15:20:35.702Z", "637479675580000000", "server-files/Users/rw/README.txt", false)).
Add(3, new File(3, "README.txt", "78", "text/plain", 16, 1, "2021-02-19T15:20:35.704Z", "2021-02-19T15:20:35.704Z", "637479675580000000", "server-files/Users/ro/README.txt", false)).
Add(4, new File(4, "INTRO.txt", "184", "text/plain", 9, 1, "2021-02-19T15:20:35.705Z", "2021-02-19T15:20:35.705Z", "637479675580000000", "server-files/Shared files/INTRO.txt", false))
];;

let directories = [Map.empty.
Add(1, new Orbit_Directory(1, "server-files", "server-files/", 1, 0, false, false)).
Add(2, new Orbit_Directory(2, "Projects", "server-files/Projects/", 1, 1, false, false)).
Add(3, new Orbit_Directory(3, "Project deliverables", "server-files/Project deliverables/", 1, 1, false, false)).
Add(4, new Orbit_Directory(4, "Project Templates", "server-files/Project Templates/", 1, 1, false, false)).
Add(5, new Orbit_Directory(5, "Standard - 1", "server-files/Project Templates/Standard - 1/", 1, 4, false, false)).
Add(6, new Orbit_Directory(6, "deliverables", "server-files/Project Templates/Standard - 1/deliverables/", 1, 5, false, false)).
Add(7, new Orbit_Directory(7, "explorer_root", "server-files/Project Templates/Standard - 1/explorer_root/", 1, 5, false, false)).
Add(8, new Orbit_Directory(8, "Project emails", "server-files/Project emails/", 1, 1, false, false)).
Add(9, new Orbit_Directory(9, "Shared files", "server-files/Shared files/", 1, 1, false, false)).
Add(10, new Orbit_Directory(10, "Companies", "server-files/Companies/", 1, 1, false, false)).
Add(11, new Orbit_Directory(11, "snapshots", "server-files/snapshots/", 1, 1, false, false)).
Add(12, new Orbit_Directory(12, "Sales Activities", "server-files/Sales Activities/", 1, 1, false, false)).
Add(13, new Orbit_Directory(13, "File importer", "server-files/File importer/", 1, 1, false, false)).
Add(14, new Orbit_Directory(14, "Users", "server-files/Users/", 1, 1, false, false)).
Add(15, new Orbit_Directory(15, "rw", "server-files/Users/rw/", 1, 14, false, false)).
Add(16, new Orbit_Directory(16, "ro", "server-files/Users/ro/", 1, 14, false, false)).
Add(17, new Orbit_Directory(17, "Project 1", "server-files/Projects/Project 1/", 1, 2, false, false)).
Add(18, new Orbit_Directory(18, "Project 2", "server-files/Projects/Project 2/", 1, 2, false, false)).
Add(19, new Orbit_Directory(19, "none", "server-files/Users/none/", 1, 14, false, false)).
Add(20, new Orbit_Directory(20, "Delete me","server-files/Shared files/Delete me/", 1, 9, false, false)).
Add(21, new Orbit_Directory(21, "Delete me too", "server-files/File importer/Delete me too/", 1, 13, false, false))
];;

let users = [Map.empty.
Add(0, new User(0, "Bypass authorization", "")).
Add(100, new User(100, "Reader/Writer", "rw")).
Add(101, new User(101, "Reader", "ro")).
Add(102, new User(102, "None", "none"))
];;



// Test for get file list.
let getFileListResponse() = 
    Request.createUrl Get "http://localhost:8085/file/list?userId=100"
    |> HttpFs.Client.getResponse
    |> run;;

let getSpecificFileResponse = 
    Request.createUrl Get "http://localhost:8085/file/meta?userId=100&id=2"
    |> HttpFs.Client.getResponse
    |> run;;

type API_File_Json = JsonProvider<"http://localhost:8085/file/meta?userId=0&id=2">
let API_File = API_File_Json.Load("http://localhost:8085/file/meta?userId=0&id=2")


// Generates a random file id to be used. Needs to be added to the model & the api.
let fileIdGenerator() = Gen.choose(2, 4) |> Gen.sample 0 1 |> Seq.exactlyOne;;

type API() =
    member __.getFiles() = getFileListResponse().statusCode   
    member __.getSpecificFile() = API_File_Json.Load("http://localhost:8085/file/meta?userId=0&id=2")

  let spec =
  let getFiles = { new Command<API, int>() with
                    // Here we retrieve the id of the API file.
                    override __.RunActual api = api.getSpecificFile().Id; api
                    // Here we retrieve the id of the Model file.
                    override __.RunModel fileId = (files.Head.Item (2)).id
                    // Here we match the two id's to see if the states of both the api and the model are equal.
                    override __.Post(api, fileId ) = api.getSpecificFile().Id = fileId  |@ sprintf "model: %i <> %A" fileId api
                    // Print actual and model.
                    override __.ToString() = "Operation: GetFile" + " " + "Value: " + (string 2) }                      
  { new ICommandGenerator<API,int> with
      member __.InitialActual = API()
      member __.InitialModel = 0
      member __.Next model = Gen.elements [getFiles]};;

  
Check.One({Config.Verbose with MaxTest = 2;}, Command.toProperty spec);;


      