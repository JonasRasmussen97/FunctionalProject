namespace FunctionalProject.Utilities

open FunctionalProject.API
open FunctionalProject.API.API
open FunctionalProject.Model.Model


module Utilities = 

    // Credits: https://stackoverflow.com/questions/2889961/f-insert-remove-item-from-list
    let rec remove i l =
        match i, l with
            | 0, x::xs -> xs
            | i, x::xs -> x::remove (i - 1) xs
            | i, [] -> failwith "index out of range"

    let getAllFileIds (list:API.FileMetaData list) = 
        list |> List.map (fun e -> e.id)

    let getAllDirectoryIds (list: API.DirectoryMetaData list) = 
        list |> List.map (fun e -> e.id)
        
    // Den her virker!
    let getModelFileById (list:API.FileMetaData list) fileId = 
        let result = list |> List.tryFind (fun e -> e.id = fileId)
        match result with 
            | Some file -> {Fail=None; Success=Some(file)} 
            | None -> {Fail=Some(NotFound); Success=None}

    let createFileModel (model:InModel) dirId userId name timeStamp = 
        let canCreateFile = if (userId <> 0) then false else true
        match canCreateFile with 
            | true -> {Fail = None; Success = Some({model with files = {id=model.currentFileId; version=1; versionChanged=1; name=name; parentId=dirId; timestamp=timeStamp}::model.files; currentFileId = model.currentFileId+1})} 
            | false -> {Fail = None; Success = None} // {model with currentFileId = model.currentFileId+1}

    let deleteFileModel (model: InModel) userId fileId = 
        let canDeleteFile = if (userId <> 0) then false else true
        match canDeleteFile with 
            | true -> {Fail = None; Success = Some({model with files = remove fileId model.files})}
            | false -> {Fail = None; Success = None} 

