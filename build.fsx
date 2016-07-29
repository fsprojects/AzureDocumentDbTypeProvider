// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.Testing

RestorePackages()

// Properties
let buildDirs = [ "AzureDocumentDbTypeProvider/bin"; "AzureDocumentDbTypeProvider.Tests/bin" ]
let testDir = "./.test/"

// Targets
Target "Clean" (fun _ ->
    trace "-----Clean previous build-----"
    CleanDirs buildDirs
    CleanDir testDir
)

Target "Default" (fun _ ->
   trace "-----Building DEFAULT-----"
)

Target "BuildTestProj"(fun _ ->
    trace "-----Build Test Project-----"
    !!("AzureDocumentDbTypeProvider.Tests\AzureDocumentDbTypeProvider.Tests.sln")
    |> MSBuildDebug testDir "Build"
    |> Log "AppBuild-Output: "
    )

Target "BuildDebug" (fun _ ->
    trace "-----Build using DEBUG configuration-----"
    !!("AzureDocumentDbTypeProvider.sln")
    |> MSBuildDebug "" "Build"
    |> Log "AppBuild-Output: ")

Target "Test" (fun _ ->
    trace "Running Tests"
    let testDlls = !! (testDir @@ "*Tests.dll")
    testDlls |> Seq.iter (fun i -> trace i)
    testDlls |> xUnit id
)

Target "BuildRelease" (fun _ ->
    trace "-----Build using RELEASE configuration-----"
    !!("AzureDocumentDbTypeProvider.sln")
    |> MSBuildRelease "" "Build"
    |> Log "AppBuild-Output: " )

Target "CreatePackage"(fun _ -> 
    trace "----Create NuGet Package ----"
    //TODO
)

"Clean"
  ==> "BuildDebug"

"Clean"
  ==> "BuildRelease"


"BuildDebug"
    ==> "BuildTestProj"
    ==> "Test"
    ==> "Default"

"BuildRelease"
    ==> "BuildTestProj"
    ==> "Test"
    ==> "CreatePackage"

RunTargetOrDefault "Default"