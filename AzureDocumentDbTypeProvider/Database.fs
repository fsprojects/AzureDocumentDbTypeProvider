module Database

open Microsoft.Azure.Documents.Client
open System
open Microsoft.Azure.Documents
open ProviderImplementation.ProvidedTypes

type DbType 
    internal(name:string) = 
    
    member __.Name with get () = name 

module DbBuilder = 
    let createDb dbName = DbType(dbName)

module DbMemberFactory = 
    let buildDbMembers (dbType:ProvidedTypeDefinition) (domainType:ProvidedTypeDefinition) (uri:string) (pwd:string) (dbName:string) = 
       dbType.AddMembersDelayed( fun () ->
            let getName = makeProvidedProperty<string> (fun args -> <@@ dbName @@>) "Name" false
            [getName]
       )


let buildDbListing acEndpoint (acKey:string) (domainType:ProvidedTypeDefinition) = 
    let createDbType acEndpoint (acKey:string) dbName = 
        let dbType = ProvidedTypeDefinition(dbName + "Db", Some typeof<DbType>, HideObjectMethods = true)
        domainType.AddMember dbType
        DbMemberFactory.buildDbMembers dbType domainType acEndpoint acKey dbName
        let dbProp = ProvidedProperty(dbName, dbType, GetterCode = (fun _ -> <@@ DbBuilder.createDb dbName @@>))
        dbProp
    
    let dbListingType = ProvidedTypeDefinition("DatabaseListing", Some typeof<obj>, HideObjectMethods = true)
    createDbType acEndpoint acKey "test1" |> dbListingType.AddMember
    domainType.AddMember(dbListingType)

    let dbListingProp = ProvidedProperty("Databases", dbListingType, GetterCode = (fun [] -> <@@ () @@>), IsStatic = true )
    
    dbListingProp
