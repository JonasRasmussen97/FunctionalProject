namespace Project

#r "nuget: Http.fs";;
open HttpFs.Client;;
open Hopac;;
#r "nuget: FSharp.Data";;
open FSharp.Data;;
#load "Model.fsx";;
#r "nuget: FSharp.Json";;
open FSharp.Json
module API = 
        // Methods for all the different Orbit API Endpoints.
        type APIFileRecord = {id: int; version: int; versionChanged: int; name: string; parentId: int; timestamp: string}
        
        let getFileById (id: int) = "http://localhost:8085/file/meta?userId=100&id=" + string id |> Request.createUrl Get |> Request.responseAsString |> run |> Json.deserialize<APIFileRecord>  
        

        // Test for get file list.
        let getFileListResponse() = 
            Request.createUrl Get "http://localhost:8085/file/list?userId=100"
            |> HttpFs.Client.getResponse
            |> run


       