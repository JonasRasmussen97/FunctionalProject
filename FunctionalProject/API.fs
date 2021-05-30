namespace FunctionalProject.API

open HttpFs.Client;;
open Hopac;;
open FSharp.Data;;
open FSharp.Json

module API =
        // Methods for all the different Orbit API Endpoints.
        type ParentId = {id: int}
        // This is required because parent_id on directories can be either null if there is no parent or have the structure of the ParentId record.
       // type DirectoryParent = Option<ParentId>
        type FileMetaData = {id: int; version: int; versionChanged: int; name: string; parentId: int; timestamp: string}
        type FileContent = {content: string}
        type ServiceFileMetaData = {id: int; name: string; size: string; mimetype: string; parent_id: int; version: int; created_at: string; modified_at: string; ms_timestamp: string; path: string; snapshots_enabled: bool}
        type DirectoryMetaData = {id: int; name: string; path: string; version: int; parent: ParentId option; is_checked_out: bool; is_default: bool}
        type FileCreation = {id: string; version: int; name: string; timestamp: string}
        type DirectoryCreation = {name: string; id: string; version: int; parentId: int; newVersions: list<int>}
        type MoveFile = {id: int; version: int; name: string}
        type FileDeleted = {success: bool}
        type element = {id: int; version: int}
        type MoveDirectory = {success: bool; newVersions: list<element>} 
        type ErrorResponse = String
        type FileResponse = FileCreation 

        type ResponseCodes = 
            | NotFound
            | Conflict
            | InvalidFilename
            | Unauthorized
            | InternalServerError
            | BadRequest
            | UnknownError
   
        type ResponseTypes<'a> = {
            Fail: ResponseCodes option
            Pass : 'a option
        }

        // Can be called by e.g. getFileById<id> if one parameter and getDirectoryById(<id1> <id2> <"name">) if multiple parameters.
        // Get Requests
        let fileMetaInformationById userId fileId = "http://localhost:8085/file/meta?userId=" + string userId + "&id=" + string fileId |> Request.createUrl Get |> Request.responseAsString |> run |> Json.deserialize<FileMetaData>  
        let fileMetaInformationByName userId dirId fileName = "http://localhost:8085/file/meta?userId=" + string userId + "&parent_id=" + string dirId + "&name=" + string fileName |> Request.createUrl Get |> Request.responseAsString |> run |> Json.deserialize<FileMetaData>  
        let downloadFile userId fileId = "http://localhost:8085/file?userId=" + string userId + "&id=" + string fileId |> Request.createUrl Get |> Request.responseAsString |> run |> Json.deserialize<FileContent>
        let versionCheck userId ver = "http://localhost:8085/version?userId=" + string userId + "&version=" + string ver |> Request.createUrl Get |> Request.responseAsString |> run
      //let getFileMetaData userId fileId = "http://localhost:8085/api/files?userId=" + string userId + "&id=" + string fileId |> Request.createUrl Get |> Request.responseAsString |> run |> Json.deserialize<FileMetaData>
      //let getDirectoryMetaData userId dirId = "http://localhost:8085/api/directories?userId=" + string userId + "&id=" + string dirId |> Request.createUrl Get |> Request.responseAsString |> run |> Json.deserialize<DirectoryMetaData>

        let fileInfo userId fileId = ("http://localhost:8085/file/meta?userId=" + string userId + "&id=" + string fileId) |> Request.createUrl Get |> getResponse |> run |> Response.readBodyAsString 
        
        let createResponseObject x =
               {
                   Fail = None
                   Pass = Some(x)
               }
      
       
        // Post Requests
        let createFile userId dirId fileName timestamp = 
            let result = 
                Request.createUrl Post ("http://localhost:8085/file?userId=" + string userId + "&parentId=" + string dirId + "&name=" + string fileName + "&timestamp=" + string timestamp) 
                |> getResponse
                |> run
            match result.statusCode with 
                | 200 -> Response.readBodyAsString result |> run |> Json.deserialize<FileCreation> |> createResponseObject 
                | 400 -> {Fail = Some(InvalidFilename); Pass = None}
                | 401 -> {Fail = Some(Unauthorized); Pass = None}
                | 404 -> {Fail = Some(NotFound); Pass = None}
                | 409 -> {Fail = Some(Conflict); Pass = None}
                | 500 -> {Fail = Some(InternalServerError); Pass = None}
        

        let getFileMetaData userId fileId = 
           let result =
            Request.createUrl Get ("http://localhost:8085/file/meta?userId=" + string userId + "&id=" + string fileId) 
            |> getResponse
            |> run
           match result.statusCode with 
            | 200 -> Response.readBodyAsString result |> run |> Json.deserialize<FileMetaData> |> createResponseObject 
            | 400 -> {Fail = Some(InvalidFilename); Pass = None}
            | 401 -> {Fail = Some(Unauthorized); Pass = None}
            | 404 -> {Fail = Some(NotFound); Pass = None}
            | 409 -> {Fail = Some(Conflict); Pass = None}
            | 500 -> {Fail = Some(UnknownError); Pass = None}
        
        let deleteFile userId fileId fileVersion =
            let result = 
                Request.createUrl Delete ("http://localhost:8085/file?userId=" + string userId + "&id=" + string fileId + "&version=" + string fileVersion)
                |> getResponse
                |> run
            match result.statusCode with 
                | 200 -> Response.readBodyAsString result |> run |> Json.deserialize<FileDeleted> |> createResponseObject 
                | 400 -> {Fail = Some(InvalidFilename); Pass = None}
                | 401 -> {Fail = Some(Unauthorized); Pass = None}
                | 404 -> {Fail = Some(NotFound); Pass = None}
                | 500 -> {Fail = Some(UnknownError); Pass = None}
                
       
        let getDirectoryMetaData userId directoryId = 
            let result =
             Request.createUrl Get ("http://localhost:8085/api/directories?userId=" + string userId + "&id=" + string directoryId) 
             |> getResponse
             |> run
            match result.statusCode with 
             | 200 -> Response.readBodyAsString result |> run |> Json.deserialize<DirectoryMetaData> |> createResponseObject 
             | 400 -> {Fail = Some(InvalidFilename); Pass = None}
             | 401 -> {Fail = Some(Unauthorized); Pass = None}
             | 404 -> {Fail = Some(NotFound); Pass = None}
             | 409 -> {Fail = Some(Conflict); Pass = None}
             | 500 -> {Fail = Some(UnknownError); Pass = None}

        let createFileAPI userId dirId fileName timestamp = "http://localhost:8085/file?userId=" + string userId + "&parentId=" + string dirId + "&name=" + string fileName + "&timestamp=" + string timestamp |> Request.createUrl Post |> Request.responseAsString |> run |> Json.deserialize<FileCreation>
        let moveFile userId fileId version parentId newFileName = "http://localhost:8085/file/move?userId=" + string userId + "&id=" + string fileId + "&version=" + string version + "&parentId=" + string parentId + "&name=" + newFileName |> Request.createUrl Post |> Request.responseAsString |> run |> Json.deserialize<MoveFile>
        let createDirectory userId parentId dirName version = "http://localhost:8085/dir?userId=" + string userId + "&parentId=" + string parentId + "&name=" + dirName + "&version=" + string version |> Request.createUrl Post |> Request.responseAsString |> run |> Json.deserialize<DirectoryCreation>
        let moveDirectory userId dirId version name parentDirId parentDirVersion = "http://localhost:8085/dir/move?userId=" + string userId + "&id=" + string dirId + "&version=" + string version + "&name=" + name + "&parentId=" + string parentDirId + "&parentVersion=" + string parentDirVersion |> Request.createUrl Post |> Request.responseAsString |> run |> Json.deserialize<MoveDirectory>


