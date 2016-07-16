module Tests

open Xunit
open FsUnit.Xunit
open FSharp.Azure.DocumentDbTypeProvider
open TestAccountConfig
let x = TestAccountConfig.accountEndpointUri

type Tp = DocumentDbTypeProvider<TestAccountConfig.accountEndpointUri,TestAccountConfig.accountKey>

[<Fact>]
let ``Can Run Tests``() = Assert.True(true)

[<Fact>]
let ``Can connect to test account``() = 
    TestAccountConfig.validateTestAccountCredentials ()
