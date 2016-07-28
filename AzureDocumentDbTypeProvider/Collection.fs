module Collection

open ProviderImplementation.ProvidedTypes
open Microsoft.Azure.Documents.Client
open System

type CollectionType 
    internal(collectionName:string, dbName:string, uri:string,key:string) = 
    let client = new DocumentClient(Uri(uri),key)
    let dbUri = UriFactory.CreateDatabaseUri(dbName)
    
    ///Collection Name
    member __.Name with get () = collectionName 

module CollectionMemberFactory = 
    let buildMembers (collectionType:ProvidedTypeDefinition) (domainType:ProvidedTypeDefinition) (uri:string) (key:string) (dbName:string) (collectionName:string) = 
        collectionType.AddMembersDelayed( fun () -> 
            //TODO
            []
        )
module CollectionBuilder = 
    let create name dbName acEndpoint acKey = 
        CollectionType(name,dbName,acEndpoint,acKey)



let buildCollectionTypes acEndpoint (acKey:string) (domainType:ProvidedTypeDefinition) dbName =
    let createCollectionType collectionName = 
        let collectionType = ProvidedTypeDefinition(collectionName + "Collection", Some typeof<CollectionType>, HideObjectMethods = true)
        domainType.AddMember collectionType
        CollectionMemberFactory.buildMembers collectionType domainType acEndpoint acKey dbName collectionName
        ProvidedProperty(dbName, collectionType, GetterCode = (fun args -> <@@ CollectionBuilder.create collectionName dbName acEndpoint acKey @@>))
        

    let collectionListingType = ProvidedTypeDefinition(dbName + "Collections",Some typeof<obj>, HideObjectMethods = true)
    domainType.AddMember collectionListingType
    let ret = ProvidedProperty("Collections", collectionListingType, GetterCode = (fun args -> <@@ () @@>)) 
    ret.AddXmlDoc(sprintf "Lists all collections contained within the %s database" dbName)
    ret