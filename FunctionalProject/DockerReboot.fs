namespace FunctionalProject.DockerReboot
open System
open System.Diagnostics
open System.Threading.Tasks

module Docker =
    
    type DockerEngine = {
        mutable stopString : string
        startString : string
        address : string
    }
    
    type CommandResult = { 
      ExitCode: int; 
      StandardOutput: string;
      StandardError: string 
    }
    
      let executeCommand executable args =
      async {
        let startInfo = ProcessStartInfo()
        startInfo.FileName <- executable
        for a in args do
          startInfo.ArgumentList.Add(a)
        startInfo.RedirectStandardOutput <- true
        startInfo.RedirectStandardError <- true
        startInfo.UseShellExecute <- false
        startInfo.CreateNoWindow <- true
        use p = new Process()
        p.StartInfo <- startInfo
        p.Start() |> ignore

        let outTask = Task.WhenAll([|
          p.StandardOutput.ReadToEndAsync();
          p.StandardError.ReadToEndAsync()
        |])

        do! p.WaitForExitAsync() |> Async.AwaitTask
        let! out = outTask |> Async.AwaitTask
        return {
          ExitCode = p.ExitCode;
          StandardOutput = out.[0];
          StandardError = out.[1]
        }
      }
    let executeShellCommand command =
       executeCommand "CMD.exe" [command]
