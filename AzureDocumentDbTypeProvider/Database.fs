module Database

open Microsoft.Azure.Documents.Client
open System
open Microsoft.Azure.Documents
open ProviderImplementation.ProvidedTypes

type ProvidedDatabaseType (sdkObj:Database) = 
    ///Name of the DocumentDb Database
    member __.Name = sdkObj.Id

    ///Gets a reference to the underlying Database object from the DocumentDb sdk
    member __.AsDatabase = sdkObj


let listDbs acEndpoint (acKey:string) = 
    let client = new DocumentClient((new Uri(acEndpoint)), acKey)
    client.CreateDatabaseQuery() |> List.ofSeq |> List.map (fun d -> ProvidedDatabaseType(d))

    
    
    

