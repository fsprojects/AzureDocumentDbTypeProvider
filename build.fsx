// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
open Fake

RestorePackages()

// Properties
let buildDirs = [ "AzureDocumentDbTypeProvider/bin"; "AzureDocumentDbTypeProvider.Tests/bin" ]

// Targets
Target "Clean" (fun _ ->
    trace "-----Clean previous build-----"
    CleanDirs buildDirs
)

Target "Default" (fun _ ->
   trace "-----Building DEFAULT-----"
)

Target "BuildDebug" (fun _ ->
    trace "-----Build using DEBUG configuration-----"
    !!("*.sln")
    |> MSBuildDebug "" "Build"
    |> Log "AppBuild-Output: ")

// Dependencies
"Clean"
  ==> "BuildDebug"
  ==> "Default"

// start build
RunTargetOrDefault "Default"