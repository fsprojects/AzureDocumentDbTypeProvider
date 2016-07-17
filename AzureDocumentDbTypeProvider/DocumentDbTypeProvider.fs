namespace ProviderImplementation

open ProviderImplementation.ProvidedTypes
open Microsoft.FSharp.Core.CompilerServices
open System.Reflection
open System
open Config
open Database

[<TypeProvider>]
type public DocumentDbTypeProvider(config: TypeProviderConfig) as this = 
    inherit TypeProviderForNamespaces()

    let thisAssembly = Assembly.GetExecutingAssembly()
    let docDbType = ProvidedTypeDefinition(thisAssembly,namespaceName,"DocumentDbTypeProvider", baseType = Some typeof<obj>)

    let initFn (typeName : string) (args : obj []) = 
        let acProvidedType = ProvidedTypeDefinition(thisAssembly, namespaceName, typeName, baseType = Some typeof<obj>)
        acProvidedType.AddMember(ProvidedConstructor(parameters = [], InvokeCode = (fun args -> <@@ null @@>)))
        
        let getDbProperties () = 
            Database.listDbs (args.[0] :?> string) (args.[1]:?> string)
            |> List.map(fun d -> new ProvidedProperty(d.Name, typeof<string>, IsStatic = true, GetterCode = (fun _ -> <@@ "Test db name" @@>)))
        acProvidedType.AddMembers(getDbProperties())
        acProvidedType

    let parameters = 
        [ ProvidedStaticParameter("accountEndPointUri", typeof<string>, String.Empty)
          ProvidedStaticParameter("accountKey", typeof<string>, String.Empty)]

    do
        docDbType.DefineStaticParameters(parameters,initFn)
        this.AddNamespace(namespaceName,[docDbType])

[<TypeProviderAssembly>]
do ()