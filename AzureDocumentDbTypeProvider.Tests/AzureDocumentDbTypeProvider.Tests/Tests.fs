module Tests

open Xunit
open FsUnit.Xunit
open FSharp.Azure.DocumentDbTypeProvider
open TestAccountConfig

type Tp = DocumentDbTypeProvider<TestAccountConfig.AccountEndpointUri, TestAccountConfig.AccountKey>

[<Fact>]
let ``Can connect to test account``() = 
    TestAccountConfig.validateTestAccountCredentials ()

[<Fact>]
let ``'Test1' database is listed``() = 
    let t1 = Tp.Databases.test1
    () //(compilation alone indicates success)
