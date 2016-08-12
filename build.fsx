// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.Testing

let authors = ["Stewart Robertson"]
let projId = "FSharp.Azure.DocumentDbTypeProvider"
let version = environVarOrDefault  "APPVEYOR_BUILD_VERSION" "0.1-alpha1"
let summary = "A prototypical type provider for the Azure DocumentDb storage platform"
let description = "The DocumentDb Type Provider provides easy access to databases, collections and documents within an Azure DocumentDb account"
let releaseNotes = "This package is still in development"
let deploymentsDir = "./.deploy/"
let buildDir = "./.build/"



let packageFiles = [
    buildDir + "AzureDocumentDbTypeProvider.dll"
    buildDir + "AzureDocumentDbTypeProvider.xml"
    buildDir + "Microsoft.Azure.Documents.Client.xml"
    buildDir + "Microsoft.Azure.Documents.Client.dll"
    buildDir + "Newtonsoft.Json.xml"
    buildDir + "Newtonsoft.Json.dll"    
]

let packageDir = "./.package/"

let buildDirs = [ "AzureDocumentDbTypeProvider/bin"; "AzureDocumentDbTypeProvider.Tests/bin"; buildDir ]
let testDir = "./.test/"


//Sets up directories used in build process
Target "SetUp" (fun _ -> 
        [buildDir;packageDir;testDir;deploymentsDir] |> Seq.iter(fun d -> ensureDirectory d)
    )   

// Targets
Target "Clean" (fun _ ->
    trace "-----Clean previous build-----"
    CleanDirs buildDirs
    CleanDirs [testDir;packageDir]
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
    |> MSBuildRelease buildDir "Build"
    |> Log "AppBuild-Output: " )

Target "SetTestAccountCreds"(fun _ ->
    let replaceFn (inputStr:string) = 
        let testAcUri = environVar "test_acc_uri"
        let testAcKey = environVar "test_acc_key"
        let findStrKey ="let AccountKey = \"\"\"{Insert your test account key here}\"\"\"" 
        let repStrKey ="let AccountKey = \"\"\"" + testAcKey + "\"\"\"" 
        let findStrUri ="let AccountEndpointUri = \"\"\"{Insert your test account endpoint uri here}\"\"\"" 
        let repStrUri ="let AccountEndpointUri = \"\"\"" + testAcUri + "\"\"\"" 
        inputStr
            .Replace(findStrKey, repStrKey )
            .Replace(findStrUri, repStrUri)
    ReplaceInFile replaceFn "AzureDocumentDbTypeProvider.Tests\AzureDocumentDbTypeProvider.Tests\TestAccountConfig.fs"
)

Target "CreatePackage"(fun _ -> 
    trace "----Create NuGet Package ----"
    CopyFiles packageDir packageFiles
    
    NuGet (fun p ->
        {p with 
            Project = projId
            Description = description
            Files = 
                packageFiles 
                |> List.map(fun f -> 
                    (f.Replace(buildDir,"") ,Some "lib/Net45",None))
            Version = version
            Summary = summary
            ReleaseNotes = releaseNotes
            OutputPath = deploymentsDir
            WorkingDir = packageDir
            Publish = false
            Authors = authors }) "Nuget/AzureDocumentDbTypeProvider.nuspec"

)

Target "BuildPackage"(fun _ -> 
    trace "----Create NuGet Package ----"
    CopyFiles packageDir packageFiles
    NuGet (fun p ->
        {p with 
            Project = projId
            Description = description
            Files = 
                packageFiles 
                |> List.map(fun f -> 
                    (f.Replace(buildDir,"") ,Some "lib/Net45",None))
            Version = version
            Summary = summary
            ReleaseNotes = releaseNotes
            OutputPath = deploymentsDir
            WorkingDir = packageDir
            Publish = false
            
            Authors = authors }) "Nuget/AzureDocumentDbTypeProvider.nuspec"

)

"Clean"
  ==> "BuildDebug"

"Clean"
  ==> "BuildRelease"

"SetUp"
    ==> "BuildDebug"
"SetUp"
    ==> "BuildRelease"
"SetUp"
    ==> "BuildTestProj"

"SetTestAccountCreds" 
    ==> "BuildTestProj"


"BuildDebug"
    ==> "BuildTestProj"
    ==> "Test"

"Test" ==> "BuildPackage"

"BuildDebug" ?=> "BuildTestProj"
"BuildRelease" ?=> "BuildTestProj"

"BuildRelease" ==> "BuildPackage"

RunTargetOrDefault "BuildPackage"