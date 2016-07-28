module Database

open Microsoft.Azure.Documents.Client
open System
open Microsoft.Azure.Documents
open ProviderImplementation.ProvidedTypes
open Collection

type DbType 
    internal(name:string, uri:string,key:string) = 
    let client = new DocumentClient(Uri(uri),key)
    let dbUri = UriFactory.CreateDatabaseUri(name)
    
    ///Database Name
    member __.Name with get () = name 

    member __.ReadDatabase = 
        client.ReadDatabaseAsync(dbUri)
        |> Async.AwaitTask
        |> Async.RunSynchronously
    
    member __.ReadDatabaseAsync ()= async {
            return! 
                client.ReadDatabaseAsync(dbUri)
                |> Async.AwaitTask
        }

module DbBuilder = 
    let createDb dbName acEndpoint acKey = 
        DbType(dbName,acEndpoint,acKey)

module DbMemberFactory = 
    let buildDbMembers (dbType:ProvidedTypeDefinition) (domainType:ProvidedTypeDefinition) (uri:string) (key:string) (dbName:string) = 
       dbType.AddMembersDelayed( fun () ->
            let containerListing = buildCollectionListing uri key domainType dbName
            [containerListing]
       )


let buildDbListing acEndpoint (acKey:string) (domainType:ProvidedTypeDefinition) = 
    let createDbType dbName = 
        let dbType = ProvidedTypeDefinition(dbName + "Db", Some typeof<DbType>, HideObjectMethods = true)
        domainType.AddMember dbType
        DbMemberFactory.buildDbMembers dbType domainType acEndpoint acKey dbName
        ProvidedProperty(dbName, dbType, GetterCode = (fun args -> <@@ DbBuilder.createDb dbName acEndpoint acKey @@>), IsStatic = false)
        
    
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
