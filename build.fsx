// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
#r "packages/Microsoft.Azure.DocumentDB/lib/net45/Microsoft.Azure.Documents.Client.dll"

open Fake
open Fake.Testing
open Fake.Git
open Microsoft.Azure.Documents.Client
open Microsoft.Azure.Documents
open System

let authors = ["Stewart Robertson"]
let projId = "FSharp.Azure.DocumentDbTypeProvider"
let version = environVarOrDefault  "APPVEYOR_BUILD_VERSION" "0.1-alpha1"
let summary = "A prototypical type provider for the Azure DocumentDb storage platform"
let description = "The DocumentDb Type Provider provides easy access to databases, collections and documents within an Azure DocumentDb account"
let releaseNotes = "This package is still in development"
let deploymentsDir = "./.deploy/"
let buildDir = "./.build/"
let binDir = "./bin/"
let testAcUri = environVar "test_acc_uri"
let testAcKey = environVar "test_acc_key"

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

// Git configuration (used for publishing documentation in gh-pages branch)
// The profile where the project is posted
let gitOwner = "stewart-r"
let gitHome = "https://github.com/" + gitOwner

// The name of the project on GitHub
let gitName = "AzureDocumentDbTypeProvider"

// The url for the raw files hosted
let gitRaw = environVarOrDefault "gitRaw" "https://raw.github.com/stewart-r"


//Sets up directories used in build process
Target "SetUp" (fun _ -> 
        [buildDir;packageDir;testDir;deploymentsDir;binDir] |> Seq.iter(fun d -> ensureDirectory d)
    )   

// Targets
Target "Clean" (fun _ ->
    trace "-----Clean previous build-----"
    CleanDirs buildDirs
    CleanDirs [testDir;packageDir;binDir]
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


let deleteDb name = 
    let client = new DocumentClient(Uri(testAcUri), testAcKey)
    let db = 
        client.CreateDatabaseQuery()
        |> Seq.tryFind(fun d -> d.Id = name)

    match db with
    | None -> ()
    | Some _ -> 
        client.DeleteDatabaseAsync( UriFactory.CreateDatabaseUri(name))
        |> Async.AwaitTask
        |> Async.RunSynchronously
        |> ignore

let createDb name =
    let client = new DocumentClient(Uri(testAcUri), testAcKey)
    client.CreateDatabaseAsync(Database(Id=name)) 
    |> Async.AwaitTask 
    |> Async.Ignore 
    |> Async.RunSynchronously

let createCollection dbId name = 
    let client = new DocumentClient(Uri(testAcUri), testAcKey)
    let collDef = new DocumentCollection(Id=name)
    let reqOpts = new RequestOptions(OfferThroughput = Nullable(400))
    client.CreateDocumentCollectionAsync(
        UriFactory.CreateDatabaseUri(dbId),
        collDef,
        reqOpts)
        |> Async.AwaitTask
        |> Async.RunSynchronously
        |> ignore

let deleteCollection dbId name = 
    let client = new DocumentClient(Uri(testAcUri), testAcKey)
    client.DeleteDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(dbId,name))
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> ignore

let fakePath = "packages" </> "FAKE" </> "tools" </> "FAKE.exe"
let fakeStartInfo script workingDirectory args fsiargs environmentVars =
    (fun (info: System.Diagnostics.ProcessStartInfo) ->
        info.FileName <- System.IO.Path.GetFullPath fakePath
        info.Arguments <- sprintf "%s --fsiargs -d:FAKE %s \"%s\"" args fsiargs script
        info.WorkingDirectory <- workingDirectory
        let setVar k v =
            info.EnvironmentVariables.[k] <- v
        for (k, v) in environmentVars do
            setVar k v
        setVar "MSBuild" msBuildExe
        setVar "GIT" Git.CommandHelper.gitPath
        setVar "FSI" fsiPath)

/// Run the given buildscript with FAKE.exe
let executeFAKEWithOutput workingDirectory script fsiargs envArgs =
    let exitCode =
        ExecProcessWithLambdas
            (fakeStartInfo script workingDirectory "" fsiargs envArgs)
            TimeSpan.MaxValue false ignore ignore
    System.Threading.Thread.Sleep 1000
    exitCode

// Documentation
let buildDocumentationTarget fsiargs target =
    trace (sprintf "Building documentation (%s), this could take some time, please wait..." target)
    let exit = executeFAKEWithOutput "docs/tools" "generate.fsx" fsiargs ["target", target]
    if exit <> 0 then
        failwith "generating reference documentation failed"
    ()

Target "GenerateReferenceDocs" (fun _ ->
    buildDocumentationTarget "-d:RELEASE -d:REFERENCE" "Default"
)

let generateHelp' fail debug =
    let args =
        if debug then "--define:HELP"
        else "--define:RELEASE --define:HELP"
    try
        buildDocumentationTarget args "Default"
        traceImportant "Help generated"
    with
    | e when not fail ->
        traceImportant "generating help documentation failed"

let generateHelp fail =
    generateHelp' fail false

Target "GenerateHelp" (fun _ ->
    DeleteFile "docs/content/release-notes.md"
    CopyFile "docs/content/" "RELEASE_NOTES.md"
    Rename "docs/content/release-notes.md" "docs/content/RELEASE_NOTES.md"

    DeleteFile "docs/content/license.md"
    CopyFile "docs/content/" "LICENSE.txt"
    Rename "docs/content/license.md" "docs/content/LICENSE.txt"

    generateHelp true
)

Target "GenerateHelpDebug" (fun _ ->
    DeleteFile "docs/content/release-notes.md"
    CopyFile "docs/content/" "RELEASE_NOTES.md"
    Rename "docs/content/release-notes.md" "docs/content/RELEASE_NOTES.md"

    DeleteFile "docs/content/license.md"
    CopyFile "docs/content/" "LICENSE.txt"
    Rename "docs/content/license.md" "docs/content/LICENSE.txt"

    generateHelp' true true
)

Target "KeepRunning" (fun _ ->
    use watcher = !! "docs/content/**/*.*" |> WatchChanges (fun changes ->
         generateHelp' true true
    )

    traceImportant "Waiting for help edits. Press any key to stop."

    System.Console.ReadKey() |> ignore

    watcher.Dispose()
)

Target "CopyBin" (fun _ -> 
    Copy binDir (!!(packageDir + "**"))
)

FinalTarget "CleanTestData" (fun _ ->
    deleteDb "test1"
    deleteDb "test2"
)

Target "InitTestData" (fun _ ->
    ActivateFinalTarget "CleanTestData"
    deleteDb "test1"
    deleteDb "test2"
    createDb "test1"
    createDb "test2"
    createCollection "test1" "TestCollection"
    )

Target "CleanDocs" (fun _ ->
    CleanDirs ["docs/output"]
)

Target "GenerateDocs" DoNothing

Target "ReleaseDocs" (fun _ ->
    let tempDocsDir = "temp" </> "gh-pages"
    ensureDirectory tempDocsDir
    CleanDir tempDocsDir
    Repository.cloneSingleBranch "" (gitHome + "/" + gitName + ".git") "gh-pages" tempDocsDir

    Git.CommandHelper.runSimpleGitCommand tempDocsDir "rm . -f -r" |> ignore
    CopyRecursive "docs/output" tempDocsDir true |> tracefn "%A"
    
    StageAll tempDocsDir
    Git.Commit.Commit tempDocsDir "Update generated documentation"
    Branches.push tempDocsDir)

"InitTestData" ==> "BuildTestProj"

"GenerateDocs" ==> "ReleaseDocs"

"CopyBin"
  ==> "GenerateHelp"
  ==> "GenerateReferenceDocs"
  ==> "GenerateDocs"

"CopyBin"
  ==> "GenerateHelpDebug"

"CleanDocs"
  ==> "GenerateHelp"
  ==> "GenerateReferenceDocs"
  ==> "GenerateDocs"

"CleanDocs"
  ==> "GenerateHelpDebug"

"BuildPackage"
  ==> "CopyBin"

"GenerateHelpDebug"
  ==> "KeepRunning"

"Clean"
  ==> "BuildDebug"

"Clean"
  ==> "BuildRelease"

"SetUp" ==> "BuildRelease"
"SetUp" ==> "BuildDebug"
"SetUp" ==> "BuildTestProj"
"SetUp" ==> "CopyBin"

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