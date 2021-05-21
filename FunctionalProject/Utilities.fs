namespace FunctionalProject.Utilities

open FunctionalProject.API
open FunctionalProject.API.API
open FunctionalProject.Model.Model


module Utilities = 


    let getAllFileIds (list:API.FileMetaData list) = 
        list |> List.map (fun e -> e.id)
            

    let createFileModel (model:InModel) fileId  userId = 
        let canCreateFile = if (userId <> 0) then false else true
        match canCreateFile with 
            | true -> {Fail = None; Success = Some({model with FileMetaData = {id=model.currentFileId; version=1; versionChanged=1; name="README.txt"; parentId=15; timestamp="637479675580000000"}::model.FileMetaData;currentFileId = model.currentFileId+1})}
            
            | false -> {Fail = None; Success = None} // {model with currentFileId = model.currentFileId+1}