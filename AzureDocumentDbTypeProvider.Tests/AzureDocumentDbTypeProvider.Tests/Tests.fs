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
let ``both test databases are listed``() = 
    let t1 = Tp.Databases.test1
    let t2 = Tp.Databases.test2
    () //(compilation alone indicates success)

[<Fact>]
let ``Can access .Name property of 'Test1' database``() = 
    let dbs = Tp.Databases
    let name = dbs.test1.Name
    Assert.Equal<string>("test1",name)

[<Fact>]
<<<<<<< HEAD
let ``Can access .Containers property of test1 database``() = 
    let test1 = Tp.Databases.test1
    let containers = test1.Containers
=======
let ``Can access .Collections property of test1 database``() = 
    let test1 = Tp.Databases.test1
    let collections = test1.Collections
>>>>>>> dev
    () //compilation alone indicates success

[<Fact>]
let ``.Containers property of test1 database contains a container for ``() = 
<<<<<<< HEAD
    let test1 = Tp.Databases.test1
    let containers = test1.Containers
=======
    let coll = Tp.Databases.test1.Collections.TestCollection
    
>>>>>>> dev
    () //compilation alone indicates success


//TODO:
//can create and delete databases
<<<<<<< HEAD
//can create, read and delete containers
=======
//can create, read and delete collections
>>>>>>> dev
