// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
open Fake

// Properties
let buildDirs = [ "AzureDocumentDbTypeProvider/bin"; "AzureDocumentDbTypeProvider.Tests/bin" ]

// Targets
Target "Clean" (fun _ ->
   CleanDirs buildDirs
)

Target "Default" (fun _ ->
   trace "Hello World from FAKE"
)

Target "BuildApp" (fun _ ->
    !!("*.sln")
    |> MSBuildRelease "" "Build"
    |> Log "AppBuild-Output: ")

// Dependencies
"Clean"
  ==> "BuildApp"
  ==> "Default"

// start build
RunTargetOrDefault "Default"