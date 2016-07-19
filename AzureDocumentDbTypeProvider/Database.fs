module Database

open Microsoft.Azure.Documents.Client
open System
open Microsoft.Azure.Documents
open ProviderImplementation.ProvidedTypes

type DbType 
    internal(sdkObj:Database) = 
    
    ///Name of the DocumentDb Database
    member __.Name
        with public get() = 
            "test1"
            //sdkObj.Id
            
        

    ///Gets a reference to the underlying Database object from the DocumentDb sdk
    member __.AsDatabase
        with public get () = sdkObj

let createDbType acEndPoint (acKey:string) (sdkObj:Database) = 
    let dbType = ProvidedTypeDefinition(sdkObj.Id, Some typeof<DbType>, HideObjectMethods = true)
    let p1 = ProvidedParameter("Database",typeof<Database>)
    dbType.AddMember(ProvidedConstructor(parameters = [p1], InvokeCode = (fun [p1] -> 
        <@@ 
            let db = ((%%p1:obj) :?> Database)
            DbType(db) 
         @@>)))
    let dbId = sdkObj.Id

    let pProp = ProvidedProperty(dbId,typeof<string>, IsStatic=true, GetterCode = fun _ ->
            <@@
                let client = new DocumentClient(Uri(acEndPoint),acKey)
                let dbObj = 
                    client.CreateDatabaseQuery() 
                    |> Seq.find(fun d -> d.Id = dbId) //FIXME - let's not list all the dbs everytime!!
                let ret = DbType(dbObj)
                ret.Name.ToString()
            @@>)
    pProp

    
    
     
    

let getDbListing acEndpoint (acKey:string) = 
    let dbListingType = ProvidedTypeDefinition("Databases",Some typeof<obj>, HideObjectMethods = true)
    ProvidedConstructor(parameters = [], InvokeCode = (fun args -> <@@ null @@>))
    |> dbListingType.AddMember

    (new DocumentClient(Uri(acEndpoint),acKey)).CreateDatabaseQuery()
    |> List.ofSeq
    |> List.map (createDbType acEndpoint acKey)
    |> dbListingType.AddMembers


    dbListingType

    
    
    

