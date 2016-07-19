module Database

open Microsoft.Azure.Documents.Client
open System
open Microsoft.Azure.Documents
open ProviderImplementation.ProvidedTypes

type DbType 
    internal(name:string) = 
    
    member __.Name with get () = name 

let getDbListing acEndpoint (acKey:string) = 
    let createDbType acEndPoint (acKey:string) dbName = 
        let dbType = ProvidedTypeDefinition(dbName + "Db", Some typeof<DbType>, HideObjectMethods = true)
        let dbProp = ProvidedProperty(dbName, dbType, GetterCode = (fun _ -> <@@ DbType(dbName) @@>))

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
    
    let dbListingType = ProvidedTypeDefinition("Databases",Some typeof<obj>, HideObjectMethods = true)
    
//    ProvidedConstructor(parameters = [], InvokeCode = (fun [] -> <@@ new DocumentClient( Uri(acEndpoint),acKey) @@>))
//    |> dbListingType.AddMember

    

    (new DocumentClient(Uri(acEndpoint),acKey)).CreateDatabaseQuery()
    |> List.ofSeq
    |> List.map (fun d -> createDbType acEndpoint acKey d.Id)
    |> dbListingType.AddMembers


    dbListingType

    
    
    

