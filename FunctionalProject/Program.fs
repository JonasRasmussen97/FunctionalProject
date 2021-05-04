namespace Program
module Program = 
open Hopac
open HttpFs.Client

let getFile =
  Request.createUrl Get "http://localhost:8085/file/list?userId=100"
  |> Request.responseAsString
  |> run


let createFile = 
  Request.createUrl Post "" 
  |> Request.responseAsString
  |> run



printfn "Here's the body: %s" getFile;;
