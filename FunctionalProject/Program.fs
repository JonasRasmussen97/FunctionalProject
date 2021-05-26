// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open FunctionalProject.Testing
open FsCheck

// Define a function to construct a message to print
let from whom =
    sprintf "from %s" whom

[<EntryPoint>]
let main argv =
    // let r = Docker.executeShellCommand "docker run -d --name orbit --rm -p8085:8085 -eCLICOLOR_FORCE=1 cr.orbit.dev/sdu/filesync-server:latest" |> Async.RunSynchronously
    Testing.start
    0 // return an integer exit code