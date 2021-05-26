namespace FunctionalProject.Utilities

open FunctionalProject.API
open FunctionalProject.API.API
open FunctionalProject.Model.Model


module Utilities = 

    let getAllFileIds (list:API.FileMetaData list) = 
        list |> List.map (fun e -> e.id)

    let getAllDirectoryIds (list: API.DirectoryMetaData list) = 
        list |> List.map (fun e -> e.id)
        
    let getModelFileById (list:API.FileMetaData list) fileId = 
        let result = list |> List.tryFind (fun e -> e.id = fileId)
        match result with 
            | Some file -> {Fail=None; Pass=Some(file)} 
            | None -> {Fail=Some(NotFound); Pass=None}
        
    let createFileModel (model:InModel) dirId userId name timeStamp = 
        let newFile = {id=model.currentFileId; version=1; versionChanged=1; name=name; parentId=dirId; timestamp=timeStamp}
        {Fail = None; Pass = Some({model with files = (newFile::model.files); currentFileId = model.currentFileId+1})} 
            
    let deleteFileModel (model: InModel) userId fileId =
        let newFiles = model.files |> List.filter(fun e -> e.id <> fileId)
        {Fail= None; Pass=Some({model with files = newFiles})}

    let getModelDirectoryById (list:API.DirectoryMetaData list) directoryId = 
        let result = list |> List.tryFind (fun e -> e.id = directoryId)
        match result with 
            | Some directory -> {Fail=None; Pass=Some(directory)}
            | None -> {Fail=Some(NotFound); Pass=None}