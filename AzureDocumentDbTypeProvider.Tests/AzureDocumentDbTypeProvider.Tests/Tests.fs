module Tests

open Xunit
open FsUnit.Xunit

[<Fact>]
let ``Can Run Tests``() = Assert.True(true)

[<Fact>]
let ``Can connect to test account``() = 
    TestAccountConfig.validateTestAccountCredentials ()
