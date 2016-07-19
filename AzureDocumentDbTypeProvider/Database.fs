module Database

open Microsoft.Azure.Documents.Client
open System
open Microsoft.Azure.Documents
open ProviderImplementation.ProvidedTypes

type DbType 
    internal(sdkObj:Database) = 

    ///Name of the DocumentDb Database
    member __.Name = sdkObj.Id

    ///Gets a reference to the underlying Database object from the DocumentDb sdk
    member __.AsDatabase = sdkObj

let createDbType (sdkObj:Database) = 
    let dbType = ProvidedTypeDefinition(sdkObj.Id, Some typeof<DbType>, HideObjectMethods = true)
    let p1 = ProvidedParameter("Database",typeof<Database>)
    dbType.AddMember(ProvidedConstructor(parameters = [p1], InvokeCode = (fun [p1] -> 
        <@@ 
            let db = ((%%p1:obj) :?> Database)
            DbType(db) 
         @@>)))

    let pProp = ProvidedProperty(sdkObj.Id,typeof<DbType>, IsStatic=true, GetterCode = fun args ->
        match args with
        | [clientParam] ->
            <@@
                let client = ((%%clientParam:obj) :?> DocumentClient)
                client.CreateDatabaseQuery() |> Seq.find(fun d -> d.Id = sdkObj.Id) //FIXME - lets not list all the dbs everytime!!
            @@>
        | _ -> <@@ null @@>)

    pProp

    
    
     
    

let getDbListing acEndpoint (acKey:string) = 
    let dbListingType = ProvidedTypeDefinition("Databases",Some typeof<obj>, HideObjectMethods = true)
    ProvidedConstructor(parameters = [], InvokeCode = (fun args -> <@@ new DocumentClient(Uri(acEndpoint),acKey) @@>))
    |> dbListingType.AddMember

    (new DocumentClient(Uri(acEndpoint),acKey)).CreateDatabaseQuery()
    |> List.ofSeq
    |> List.map createDbType
    |> dbListingType.AddMembers


    dbListingType

    
    
    

