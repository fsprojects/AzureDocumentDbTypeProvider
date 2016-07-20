namespace ProviderImplementation

open ProviderImplementation.ProvidedTypes
open ProviderImplementation.ProvidedTypes.ProvidedTypesHelpers
open Microsoft.FSharp.Core.CompilerServices
open System.Reflection
open System
open Config
open Database
open Microsoft.Azure.Documents.Client

[<TypeProvider>]
type public DocumentDbTypeProvider(config: TypeProviderConfig) as this = 
    inherit TypeProviderForNamespaces()

    let thisAssembly = Assembly.GetExecutingAssembly()
    let docDbType = ProvidedTypeDefinition(thisAssembly,namespaceName,"DocumentDbTypeProvider", baseType = Some typeof<obj>)

    let initFn (typeName : string) (args : obj []) = 
        let acEndpoint, acKey = (args.[0] :?> string), (args.[1]:?> string) 

        let acProvidedType = ProvidedTypeDefinition(thisAssembly, namespaceName, typeName, baseType = Some typeof<obj>)
        acProvidedType.AddMember(ProvidedConstructor(parameters = [], InvokeCode = (fun args -> <@@ null @@>)))
        
        let docDbClient = new ProvidedProperty("DocumentClient", typeof<DocumentClient>, IsStatic = true, GetterCode = (fun _ -> <@@ new DocumentClient(Uri(acEndpoint),acKey) @@>))
        docDbClient.AddXmlDoc "Gets a DocumentDb SDK client object for this connection"
        acProvidedType.AddMember(docDbClient)

        addDbListing acEndpoint acKey acProvidedType
        

    let parameters = 
        [ ProvidedStaticParameter("accountEndPointUri", typeof<string>, String.Empty)
          ProvidedStaticParameter("accountKey", typeof<string>, String.Empty)]

    do
        docDbType.DefineStaticParameters(parameters,initFn)
        docDbType.AddXmlDoc("The entry type to connect to Azure DocumentDb")
        this.AddNamespace(namespaceName,[docDbType])

[<TypeProviderAssembly>]
do ()