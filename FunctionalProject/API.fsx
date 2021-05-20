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
        type ParentId = {id: int}
        // This is required because parent_id on directories can be either null if there is no parent or have the structure of the ParentId record.
        type DirectoryParent = Option<ParentId>
        type FileMetaData = {id: int; version: int; versionChanged: int; name: string; parentId: int; timestamp: string}
        type FileContent = {content: string}
        type ServiceFileMetaData = {id: int; name: string; size: string; mimetype: string; parent_id: int; version: int; created_at: string; modified_at: string; ms_timestamp: string; path: string; snapshots_enabled: bool}
        
        type DirectoryMetaData = {id: int; name: string; path: string; version: int; parent: DirectoryParent; is_checked_out: bool; is_default: bool}
        
        type FileCreation = {id: string; version: int; name: string; timestamp: int64}
        type DirectoryCreation = {name: string; id: string; version: int; parentId: int; newVersions: list<int>}
        type MoveFile = {id: int; version: int; name: string}
        
        type element = {id: int; version: int}
        type MoveDirectory = {success: bool; newVersions: list<element>} 
        type ErrorResponse = String
        type FileResponse = FileCreation | ErrorResponse 
        // Can be called by e.g. getFileById<id> if one parameter and getDirectoryById(<id1> <id2> <"name">) if multiple parameters.
        // Get Requests
        let fileMetaInformationById (userId: int, fileId: int) = "http://localhost:8085/file/meta?userId=" + string userId + "&id=" + string fileId |> Request.createUrl Get |> Request.responseAsString |> run |> Json.deserialize<FileMetaData>  
        let fileMetaInformationByName (userId: int, dirId: int, fileName: string) = "http://localhost:8085/file/meta?userId=" + string userId + "&parent_id=" + string dirId + "&name=" + string fileName |> Request.createUrl Get |> Request.responseAsString |> run |> Json.deserialize<FileMetaData>  
        let downloadFile (userId: int, fileId: int) = "http://localhost:8085/file?userId=" + string userId + "&id=" + string fileId |> Request.createUrl Get |> Request.responseAsString |> run |> Json.deserialize<FileContent>
        let versionCheck (userId: int, ver: string) = "http://localhost:8085/version?userId=" + string userId + "&version=" + string ver |> Request.createUrl Get |> Request.responseAsString |> run
        let getFileMetaData (userId: int, fileId: int) = "http://localhost:8085/api/files?userId=" + string userId + "&id=" + string fileId |> Request.createUrl Get |> Request.responseAsString |> run |> Json.deserialize<ServiceFileMetaData>
        let getDirectoryMetaData (userId: int, dirId: int) = "http://localhost:8085/api/directories?userId=" + string userId + "&id=" + string dirId |> Request.createUrl Get |> Request.responseAsString |> run |> Json.deserialize<DirectoryMetaData>

        // Post Requests
        
        let create userId dirId fileName timestamp = 
            let result = 
                Request.createUrl Post ("http://localhost:8085/file?userId=" + string userId + "&parentId=" + string dirId + "&name=" + string fileName + "&timestamp=" + string timestamp) 
                |> getResponse
                |> run
            match result.statusCode with 
                | 200 -> Json.deserialize<FileResponse>
 

        
        
        let createFile (userId: int, dirId: int, fileName: string, timestamp: string) = "http://localhost:8085/file?userId=" + string userId + "&parentId=" + string dirId + "&name=" + string fileName + "&timestamp=" + string timestamp |> Request.createUrl Post |> Request.responseAsString |> run |> Json.deserialize<FileCreation>
        
        let moveFile (userId: int, fileId: int, version: int, parentId: int, newFileName: string) = "http://localhost:8085/file/move?userId=" + string userId + "&id=" + string fileId + "&version=" + string version + "&parentId=" + string parentId + "&name=" + newFileName |> Request.createUrl Post |> Request.responseAsString |> run |> Json.deserialize<MoveFile>
        let createDirectory (userId: int, parentId: int, dirName: string, version: int) = "http://localhost:8085/dir?userId=" + string userId + "&parentId=" + string parentId + "&name=" + dirName + "&version=" + string version |> Request.createUrl Post |> Request.responseAsString |> run |> Json.deserialize<DirectoryCreation>
        let moveDirectory (userId: int, dirId: int, version: int, name: string, parentDirId: int, parentDirVersion: int) = "http://localhost:8085/dir/move?userId=" + string userId + "&id=" + string dirId + "&version=" + string version + "&name=" + name + "&parentId=" + string parentDirId + "&parentVersion=" + string parentDirVersion |> Request.createUrl Post |> Request.responseAsString |> run |> Json.deserialize<MoveDirectory>

        // Test for get file list
        let getFileListResponse() = 
            Request.createUrl Get "http://localhost:8085/file/list?userId=100"
            |> HttpFs.Client.getResponse
            |> run