namespace FunctionalProject.Model

open FunctionalProject.API

module Model =

    [<StructuredFormatDisplay("{DisplayValue}")>]
    type InModel = {
        files: API.FileMetaData list
        directories : API.DirectoryMetaData list
        currentFileId: int
    }

    type InModel with
        override printModel.ToString() =
            let files = (printModel.files |> List.map (fun e -> "\n[id:" + (string e.id) + " name:" + (string e.name) + " parentId:" + (string e.parentId) + "]"  )) |> List.fold (+) ""
            sprintf "\nfiles:%s - currentFileId:%i" files printModel.currentFileId
        member t.DisplayValue = t.ToString()

    // Defines the structure of the initial model
    let model = {
        files = [
        {id=2; version=1; versionChanged=1; name="README.txt"; parentId=15; timestamp="637479675580000000"}
        {id=3; version=1; versionChanged=1; name="README.txt"; parentId=16; timestamp="637479675580000000"}
        {id=4; version=1; versionChanged=1; name="INTRO.txt"; parentId=9; timestamp="637479675580000000"}
        ]
        directories = [ 
        {id=1; name="server-files"; path="server-files/"; version=1; parent=None; is_checked_out=false; is_default=false}
        {id=2; name="Projects"; path="server-files/Projects/"; version=1; parent=Some({id=(1)}); is_checked_out=false; is_default=false}
        {id=3; name="Project deliverables"; path="server-files/Project deliverables/"; version=1; parent=Some({id=(1)}); is_checked_out=false; is_default=false}
        {id=4; name="Project Templates"; path="server-files/Project Templates/"; version=1; parent=Some({id=(1)}); is_checked_out=false; is_default=false}
        {id=5; name="Standard - 1"; path="server-files/Project Templates/Standard - 1/"; version=1; parent=Some({id=(4)}); is_checked_out=false; is_default=false}
        {id=6; name="deliverables"; path="server-files/Project Templates/Standard - 1/deliverables/"; version=1; parent=Some({id=(5)}); is_checked_out=false; is_default=false}
        {id=7; name="explorer_root"; path="server-files/Project Templates/Standard - 1/explorer_root/"; version=1; parent=Some({id=(5)}); is_checked_out=false; is_default=false}
        {id=8; name="Project emails"; path="server-files/Project emails/"; version=1; parent=Some({id=(1)}); is_checked_out=false; is_default=false}
        {id=9; name="Shared files"; path="server-files/Shared files/"; version=1; parent=Some({id=(1)}); is_checked_out=false; is_default=false}
        {id=10; name="Companies"; path="server-files/Companies/"; version=1; parent=Some({id=(1)}); is_checked_out=false; is_default=false}
        {id=11; name="snapshots"; path="server-files/snapshots/"; version=1; parent=Some({id=(1)}); is_checked_out=false; is_default=false}
        {id=12; name="Sales Activities"; path="server-files/Sales Activities/"; version=1; parent=Some({id=(1)}); is_checked_out=false; is_default=false}
        {id=13; name="File importer"; path="server-files/File importer/"; version=1; parent=Some({id=(1)}); is_checked_out=false; is_default=false}
        {id=14; name="Users"; path="server-files/Users/"; version=1; parent=Some({id=(1)}); is_checked_out=false; is_default=false}
        {id=15; name="rw"; path="server-files/Users/rw/"; version=1; parent=Some({id=(14)}); is_checked_out=false; is_default=false}
        {id=16; name="ro"; path="server-files/Users/ro/"; version=1; parent=Some({id=(14)}); is_checked_out=false; is_default=false}
        {id=17; name="Project 1"; path="server-files/Projects/Project 1/"; version=1; parent=Some({id=(2)}); is_checked_out=false; is_default=false}
        {id=18; name="Project 2"; path="server-files/Projects/Project 2/"; version=1; parent=Some({id=(2)}); is_checked_out=false; is_default=false}
        {id=19; name="none"; path="server-files/Users/none/"; version=1; parent=Some({id=(14)}); is_checked_out=false; is_default=false}
        {id=20; name="Delete me"; path="server-files/Shared files/Delete me/"; version=1; parent=Some({id=(9)}); is_checked_out=false; is_default=false}
        {id=21; name="Delete me too"; path="server-files/File importer/Delete me too/"; version=1; parent=Some({id=(13)}); is_checked_out=false; is_default=false}
        ]
        currentFileId = 5
    }