namespace Project
    module Model =

        // Defines the structure of a file.
        type File = { id: int; version: int; versionChanged: int; name: string; parentId: int; timestamp: string; }
        // Required as the parent_id is another record.
        type directoryParent = {id: int}
        // Defines the structure of a directory.
        type OrbitDirectory = {id: int; name: string; path: string; version: int; parent_id: directoryParent; is_checked_out: bool; is_default: bool}
        // Defines a file response from the service part of the API.
        type ServiceFile = {id: int; name: string; size: string; mimetype: string; parent_id: directoryParent; version: int; created_at: string; modified_at: string; ms_timestamp: string; path: string; snapshots_enabled: bool}
        // Defines the structure of a user.
        type User = {id: int; name: string; initials: string}
        
       
        
        // All files NOT from the service request, they follow another format.
        let files = [Map.empty.
        Add(2, {id=2; version=1; versionChanged=1; name="README.txt"; parentId=15; timestamp="637479675580000000"}).
        Add(3, {id=3; version=1; versionChanged=1; name="README.txt"; parentId=16; timestamp="637479675580000000"}).
        Add(4, {id=4; version=1; versionChanged=1; name="INTRO.txt"; parentId=9; timestamp="637479675580000000"})
        ]

        let directories = [Map.empty.
        Add(1, {id=1; name="server-files"; path="server-files/"; version=1; parent_id={id=0}; is_checked_out=false; is_default=false}).
        Add(2, {id=2; name="Projects"; path="server-files/Projects/"; version=1; parent_id={id=1}; is_checked_out=false; is_default=false}).
        Add(3, {id=3; name="Project deliverables"; path="server-files/Project deliverables/"; version=1; parent_id={id=1}; is_checked_out=false; is_default=false}).
        Add(4, {id=4; name="Project Templates"; path="server-files/Project Templates/"; version=1; parent_id={id=1}; is_checked_out=false; is_default=false}).
        Add(5, {id=5; name="Standard - 1"; path="server-files/Project Templates/Standard - 1/"; version=1; parent_id={id=4}; is_checked_out=false; is_default=false}).
        Add(6, {id=6; name="deliverables"; path="server-files/Project Templates/Standard - 1/deliverables/"; version=1; parent_id={id=5}; is_checked_out=false; is_default=false}).
        Add(7, {id=7; name="explorer_root"; path="server-files/Project Templates/Standard - 1/explorer_root/"; version=1; parent_id={id=5}; is_checked_out=false; is_default=false}).
        Add(8, {id=8; name="Project emails"; path="server-files/Project emails/"; version=1; parent_id={id=1}; is_checked_out=false; is_default=false}).
        Add(9, {id=9; name="Shared files"; path="server-files/Shared files/"; version=1; parent_id={id=1}; is_checked_out=false; is_default=false}).
        Add(10, {id=10; name="Companies"; path="server-files/Companies/"; version=1; parent_id={id=1}; is_checked_out=false; is_default=false}).
        Add(11, {id=11; name="snapshots"; path="server-files/snapshots/"; version=1; parent_id={id=1}; is_checked_out=false; is_default=false}).
        Add(12, {id=12; name="Sales Activities"; path="server-files/Sales Activities/"; version=1; parent_id={id=1}; is_checked_out=false; is_default=false}).
        Add(13, {id=13; name="File importer"; path="server-files/File importer/"; version=1; parent_id={id=1}; is_checked_out=false; is_default=false}).
        Add(14, {id=14; name="Users"; path="server-files/Users/"; version=1; parent_id={id=1}; is_checked_out=false; is_default=false}).
        Add(15, {id=15; name="rw"; path="server-files/Users/rw/"; version=1; parent_id={id=14}; is_checked_out=false; is_default=false}).
        Add(16, {id=16; name="ro"; path="server-files/Users/ro/"; version=1; parent_id={id=14}; is_checked_out=false; is_default=false}).
        Add(17, {id=17; name="Project 1"; path="server-files/Projects/Project 1/"; version=1; parent_id={id=2}; is_checked_out=false; is_default=false}).
        Add(18, {id=18; name="Project 2"; path="server-files/Projects/Project 2/"; version=1; parent_id={id=2}; is_checked_out=false; is_default=false}).
        Add(19, {id=19; name="none"; path="server-files/Users/none/"; version=1; parent_id={id=14}; is_checked_out=false; is_default=false}).
        Add(20, {id=20; name="Delete me"; path="server-files/Shared files/Delete me/"; version=1; parent_id={id=9}; is_checked_out=false; is_default=false}).
        Add(21, {id=21; name="Delete me too"; path="server-files/File importer/Delete me too/"; version=1; parent_id={id=13}; is_checked_out=false; is_default=false})
        ]

        let users = [Map.empty.
        Add(0, {id=0; name="Bypass authorization"; initials=""}).
        Add(100, {id=100; name="Reader/Writer"; initials="rw"}).
        Add(101, {id=101; name="Reader"; initials="ro"}).
        Add(102, {id=102; name="None"; initials="none"})
        ];;
