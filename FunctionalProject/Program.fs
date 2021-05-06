namespace Program
module Program = 
#r "nuget: FSharp.Json"
#r "nuget: FSharp.Data"
open Hopac
open FSharp.Data
open HttpFs.Client
open System.Text.Json
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

let files = [Map.empty.
Add(2, new File(2, "README.txt", "78", "text/plain", 15, 1, "2021-02-19T15:20:35.702Z", "2021-02-19T15:20:35.702Z", "637479675580000000", "server-files/Users/rw/README.txt", false)).
Add(3, new File(3, "README.txt", "78", "text/plain", 16, 1, "2021-02-19T15:20:35.704Z", "2021-02-19T15:20:35.704Z", "637479675580000000", "server-files/Users/ro/README.txt", false)).
Add(4, new File(4, "INTRO.txt", "184", "text/plain", 9, 1, "2021-02-19T15:20:35.705Z", "2021-02-19T15:20:35.705Z", "637479675580000000", "server-files/Shared files/INTRO.txt", false))
]

let getFile = 
    Request.createUrl Get "http://localhost:8085/file/meta?userId=0&id=2"
    |> Request.setHeader (ContentType (ContentType.create("application", "json")))  //Reading content of json body
    |> Request.responseAsString
    |> run

