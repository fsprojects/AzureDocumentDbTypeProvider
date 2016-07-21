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
    let t1 = Tp.Databases
    () //(compilation alone indicates success)

[<Fact>]
let ``Can access .Name property of 'Test1' database``() = 
    let dbs = Tp.Databases
    let name = dbs.test1.Name
    Assert.Equal<string>("test1",name)
    