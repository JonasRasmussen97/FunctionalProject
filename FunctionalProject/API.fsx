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
        // Can be called by e.g. getFileById<id> if one parameter and getDirectoryById(<id1> <id2> <"name">) if multiple parameters.

        type FileMetaData = {id: int; version: int; versionChanged: int; name: string; parentId: int; timestamp: string}
        type FileContent = {content: string}
        type ServiceFileMetaData = {id: int; name: string; size: string; mimetype: string; parent_id: int; version: int; created_at: string; modified_at: string; ms_timestamp: string; path: string; snapshots_enabled: bool}
        
        type ParentId = {id: int}
        type DirectoryMetaData = {id: int; name: string; path: string; version: int; parent: ParentId; isCheckedOut: bool; isDefault: bool}
        
        // Get Requests
        let fileMetaInformationById (userId: int, fileId: int) = "http://localhost:8085/file/meta?userId=" + string userId + "&id=" + string fileId |> Request.createUrl Get |> Request.responseAsString |> run |> Json.deserialize<FileMetaData>  
        let fileMetaInformationByName (userId: int, dirId: int, fileName: string) = "http://localhost:8085/file/meta?userId=" + string userId + "&parent_id=" + string dirId + "&name=" + string fileName |> Request.createUrl Get |> Request.responseAsString |> run |> Json.deserialize<FileMetaData>  
        let downloadFile (userId: int, fileId: int) = "http://localhost:8085/file?userId=" + string userId + "&id=" + string fileId |> Request.createUrl Get |> Request.responseAsString |> run |> Json.deserialize<FileContent>
        let versionCheck (userId: int, ver: string) = "http://localhost:8085/version?userId=" + string userId + "&version=" + string ver |> Request.createUrl Get |> Request.responseAsString |> run
        let getFileMetaData (userId: int, fileId: int) = "http://localhost:8085/api/files?userId=" + string userId + "&id=" + string fileId |> Request.createUrl Get |> Request.responseAsString |> run |> Json.deserialize<ServiceFileMetaData>
        let getDirectoryMetaData (userId: int, dirId: int) = "http://localhost:8085/api/directories?userId=" + string userId + "&id=" + string dirId |> Request.createUrl Get |> Request.responseAsString |> run |> Json.deserialize<DirectoryMetaData>

        // Create Requests
        

        // Test for get file list.
        let getFileListResponse() = 
            Request.createUrl Get "http://localhost:8085/file/list?userId=100"
            |> HttpFs.Client.getResponse
            |> run