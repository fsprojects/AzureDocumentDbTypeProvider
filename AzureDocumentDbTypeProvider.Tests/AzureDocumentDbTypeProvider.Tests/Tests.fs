module Tests

open Xunit
open FsUnit.Xunit
open FSharp.Azure.DocumentDbTypeProvider
open TestAccountConfig

type Tp = DocumentDbTypeProvider<TestAccountConfig.accountEndpointUri,TestAccountConfig.accountKey>


[<Fact>]
let ``Can Run Tests``() = Assert.True(true)

[<Fact>]
let ``Can connect to test account``() = 
    TestAccountConfig.validateTestAccountCredentials ()

let ``'Test1' database is listed``() = 
    let t1 = Tp.test1
    ()
