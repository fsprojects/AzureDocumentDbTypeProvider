module Database

open Microsoft.Azure.Documents.Client
open System
open Microsoft.Azure.Documents
open ProviderImplementation.ProvidedTypes

type DbType 
    internal(name:string, uri:string,key:string) = 
    ///Database Name
    member __.Name with get () = name 

module DbBuilder = 
    let createDb dbName acEndpoint acKey = 
        DbType(dbName,acEndpoint,acKey)

module DbMemberFactory = 
    let buildDbMembers (dbType:ProvidedTypeDefinition) (domainType:ProvidedTypeDefinition) (uri:string) (pwd:string) (dbName:string) = 
       dbType.AddMembersDelayed( fun () ->
            let getName = makeProvidedProperty<string> (fun args -> <@@ dbName @@>) "Name" false
            [getName]
       )


let buildDbListing acEndpoint (acKey:string) (domainType:ProvidedTypeDefinition) = 
    let createDbType dbName = 
        let dbType = ProvidedTypeDefinition(dbName + "Db", Some typeof<DbType>, HideObjectMethods = true)
        domainType.AddMember dbType
        DbMemberFactory.buildDbMembers dbType domainType acEndpoint acKey dbName
        let dbProp = ProvidedProperty(dbName, dbType, GetterCode = (fun _ -> <@@ DbBuilder.createDb dbName acEndpoint acKey @@>))
        dbProp
    
    let dbListingType = ProvidedTypeDefinition("DatabaseListing", Some typeof<obj>, HideObjectMethods = true)
    dbListingType.AddXmlDoc("Lists all databases in the DocumentDb account")
    let propGenFn = 
        (fun () -> 
            new DocumentClient(Uri(acEndpoint),acKey)
            |> (fun d -> d.CreateDatabaseQuery())
            |> Seq.map(fun d -> createDbType d.Id )
            |> List.ofSeq)
    dbListingType.AddMembersDelayed propGenFn
    
    domainType.AddMember(dbListingType)

    let dbListingProp = ProvidedProperty("Databases", dbListingType, GetterCode = (fun [] -> <@@ () @@>), IsStatic = true )
    
    dbListingProp
