﻿// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open FunctionalProject.Testing
open FsCheck
open System.Threading

// Define a function to construct a message to print
let from whom =
    sprintf "from %s" whom

[<EntryPoint>]
let main argv =
    Testing.start
    0 // return an integer exit code