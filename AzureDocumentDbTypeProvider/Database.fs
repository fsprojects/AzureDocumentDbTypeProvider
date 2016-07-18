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
    dbType.AddMember(ProvidedConstructor(parameters = [], InvokeCode = (fun args -> <@@ null @@>)))
    let nameProp = ProvidedProperty("Name",typeof<string>,GetterCode = fun _ -> <@@ sdkObj.Id @@>)
    
    dbType.AddMember(nameProp)
    dbType

let listDbs acEndpoint (acKey:string) = 
    let client = new DocumentClient((new Uri(acEndpoint)), acKey)
    client.CreateDatabaseQuery() 
    |> List.ofSeq 
    |> List.map createDbType

    
    
    

