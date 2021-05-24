namespace FunctionalProject.Utilities

open FunctionalProject.API
open FunctionalProject.API.API
open FunctionalProject.Model.Model


module Utilities = 

    // Credits: https://stackoverflow.com/questions/2889961/f-insert-remove-item-from-list
    let rec remove index list =
        match index, list with
            | 0, head::restOfArr -> restOfArr
            | index, head::restOfArr -> head::remove (index - 1) restOfArr
            | index, [] -> failwith "index out of range"

    let findIndexById fileId list = 
        let res = List.findIndex<FileMetaData> (fun e -> e.id = fileId) list
        res

    let getAllFileIds (list:API.FileMetaData list) = 
        list |> List.map (fun e -> e.id)
    
    let getFileMetaDataModel (model:InModel) userId fileId = 
        let canGetFile = if (userId <> 0) then false else true
        match canGetFile with 
            | true -> {Fail = None; Success = Some(model.files.Item (findIndexById fileId model.files) )} 
            | false -> {Fail = None; Success = None}

    let createFileModel (model:InModel) dirId  userId name timeStamp = 
        let canCreateFile = if (userId <> 0) then false else true
        match canCreateFile with 
            | true -> {Fail = None; Success = Some({model with files = {id=model.currentFileId; version=1; versionChanged=1; name=name; parentId=dirId; timestamp=timeStamp}::model.files;currentFileId = model.currentFileId+1})} 
            | false -> {Fail = None; Success = None} // {model with currentFileId = model.currentFileId+1}

    let deleteFileModel (model: InModel) userId fileId = 
        let canDeleteFile = if (userId <> 0) then false else true
        match canDeleteFile with 
            | true -> {Fail = None; Success = Some({model with files = remove fileId model.files})}
            | false -> {Fail = None; Success = None} 

