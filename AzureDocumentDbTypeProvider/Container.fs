module Container

open ProviderImplementation.ProvidedTypes
open Microsoft.Azure.Documents.Client
open System

type ContainerType 
    internal(containerName:string, dbName:string, uri:string,key:string) = 
    let client = new DocumentClient(Uri(uri),key)
    let dbUri = UriFactory.CreateDatabaseUri(dbName)
    
    ///Container Name
    member __.Name with get () = containerName 

module ContainerMemberFactory = 
    let buildMembers (containerType:ProvidedTypeDefinition) (domainType:ProvidedTypeDefinition) (uri:string) (key:string) (dbName:string) (containerName:string) = 
        containerType.AddMembersDelayed( fun () -> 
            //TODO
            []
        )
module ContainerBuilder = 
    let create name dbName acEndpoint acKey = 
        ContainerType(name,dbName,acEndpoint,acKey)



let buildContainerTypes acEndpoint (acKey:string) (domainType:ProvidedTypeDefinition) dbName =
    let createContainerType containerName = 
        let containerType = ProvidedTypeDefinition(containerName + "Container", Some typeof<ContainerType>, HideObjectMethods = true)
        domainType.AddMember containerType
        ContainerMemberFactory.buildMembers containerType domainType acEndpoint acKey dbName containerName
        ProvidedProperty(dbName, containerType, GetterCode = (fun args -> <@@ ContainerBuilder.create containerName dbName acEndpoint acKey @@>))
        

    let containerListingType = ProvidedTypeDefinition(dbName + "Containers",Some typeof<obj>, HideObjectMethods = true)
    domainType.AddMember containerListingType
    let ret = ProvidedProperty("Containers", containerListingType, GetterCode = (fun args -> <@@ () @@>)) 
    ret.AddXmlDoc(sprintf "Lists all containers contained within the %s database" dbName)
    ret