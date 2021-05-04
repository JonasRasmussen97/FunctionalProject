namespace Model
#r "nuget: Http.fs"
open HttpFs.Client
open Hopac
open FsCheck
module OrbitModel = 
// Defines a file type as it is according to the Orbit API.
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


// Values that define the state of the system. They follow the API.
let directories = Map.empty
let files = Map.empty
// users.TryFind "value here" and users.Count returns the length.
let users = Map.empty.Add(0, new User(0, "Bypass authorization", "")).Add(100, new User(100, "Reader/Writer", "rw")).Add(101, new User(101, "Reader", "ro")).Add(102, new User(102, "None", "none"));;

let response = Request.createUrl Get "http://localhost:8085/file/list?userId=100" |> HttpFs.Client.getResponse |> run;;
// Class definition of the API
type API() = 
    member __.getFiles() = let response = Request.createUrl Get "http://localhost:8085/file/list?userId=100" |> HttpFs.Client.getResponse |> run;;
    




let spec = 
    let _getFiles = { new Command<API, int>() with 
        override __.RunActual api = api.getFiles().statusCode; api 
        override __.RunModel m = 200
    }

{
    new ICommandGenerator<API, int> with {
        member __.InitialActual = API()
        member __.InitialModel = 0
        member __.Next model = Gen.elements [_getFiles]
    }
}