module Database

open Microsoft.Azure.Documents.Client
open System
open Microsoft.Azure.Documents
open ProviderImplementation.ProvidedTypes

type DbType 
    internal(name:string) = 
    
    member __.Name with get () = name 

let addDbListing acEndpoint (acKey:string) (domainType:ProvidedTypeDefinition) = 
    let dbListingType = ProvidedTypeDefinition("DatabaseListing", Some typeof<obj>, HideObjectMethods = true)
    domainType.AddMember dbListingType

    let prop = ProvidedProperty("Databases", dbListingType, GetterCode = (fun [] -> <@@ () @@>), IsStatic = true )
    let createDbType dbName = 
        let dbType = ProvidedTypeDefinition(dbName + "Db", Some typeof<DbType>, HideObjectMethods = true)
        let dbProp = ProvidedProperty(dbName, typeof<string>, GetterCode = (fun _ -> <@@ dbName @@>), IsStatic=true)

//        let pProp = ProvidedProperty(dbName,typeof<string>, IsStatic=false, GetterCode = fun [baseClient] ->
//    //    let pMethod = ProvidedMethod(dbId,[],typeof<string>, IsStatic = true, InvokeCode = fun [baseClient] ->
//                <@@
//                    let client = ((%%baseClient:obj) :?> DocumentClient)
//                    let dbObj = 
//                        client.CreateDatabaseQuery() 
//                        |> Seq.find(fun d -> d.Id = dbId) //FIXME - let's not list all the dbs everytime!!
//    //                let ret = DbType(dbObj)
//                    dbObj.Id
//                @@>)
        dbProp
    

    makeProvidedProperty<string> (fun _ -> <@@ "test1" @@>) "test1" true
    |> dbListingType.AddMember
    
//    ProvidedConstructor(parameters = [], InvokeCode = (fun [] -> <@@ new DocumentClient( Uri(acEndpoint),acKey) @@>))
//    |> dbListingType.AddMember

    
//
//    (new DocumentClient(Uri(acEndpoint),acKey)).CreateDatabaseQuery()
//    |> List.ofSeq
//    |> List.map (fun d -> createDbType d.Id)
//    |> dbListingType.AddMembers


    dbListingType

    
    
    

