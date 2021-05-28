namespace FunctionalProject.Utilities

open FunctionalProject.API
open FunctionalProject.API.API
open FunctionalProject.Model.Model


module Utilities = 

    let rec checkIfFileExistsInDir (list:FileMetaData list) fileName dirId = match list with
        | file::files -> if (file.parentId <> dirId && file.name = fileName) then Some(file) else checkIfFileExistsInDir files fileName dirId
        | [] -> None

    let getAllFileIds (list:API.FileMetaData list) = 
        list |> List.map (fun e -> e.id)

    let getAllDirectoryIds (list: API.DirectoryMetaData list) = 
        list |> List.map (fun e -> e.id)
        
    let getModelFileById (list:API.FileMetaData list) fileId = 
        let result = list |> List.tryFind (fun e -> e.id = fileId)
        match result with 
            | Some file -> {Fail=None; Pass=Some(file)} 
            | None -> {Fail=Some(NotFound); Pass=None}
            
    let createFileModel (model:InModel) dirId 0 name timeStamp = 
        let newFile = {id=model.currentFileId; version=1; versionChanged=1; name=name; parentId=dirId; timestamp=timeStamp}
        let result = checkIfFileExistsInDir model.files name dirId
        match result with 
            | Some file -> {Fail = Some(Conflict); Pass=None}
            | None -> {Fail = None; Pass = Some({model with files = newFile::model.files; currentFileId = model.currentFileId+1})} 

            
    let deleteFileModel (model: InModel) userId fileId =
        let fileExists = model.files |> List.tryFind (fun e -> e.id = fileId)
        let fileExistsResult = match fileExists with 
            | Some file -> true 
            | None -> false

        let newFiles = model.files |> List.filter(fun e -> e.id <> fileId)
        let fileLength = newFiles.Length
        match fileExistsResult, fileLength with 
            | true, 0 -> {Fail = Some(NotFound); Pass=None}
            | true, _ -> {Fail= None; Pass=Some({model with files = newFiles})}
            | false, _ -> {Fail = Some(NotFound); Pass=None}
            
    let getModelDirectoryById (list:API.DirectoryMetaData list) directoryId = 
        let result = list |> List.tryFind (fun e -> e.id = directoryId)
        match result with 
            | Some directory -> {Fail=None; Pass=Some(directory)}
            | None -> {Fail=Some(NotFound); Pass=None}