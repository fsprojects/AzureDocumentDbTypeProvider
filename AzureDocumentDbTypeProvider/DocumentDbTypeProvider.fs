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
//    
//    let handler = System.ResolveEventHandler(fun _ args ->
//        let asmName = AssemblyName(args.Name)
//        // assuming that we reference only dll files
//        let expectedName = asmName.Name + ".dll"
//        let expectedLocation =
//            // we expect to find this assembly near the dll with type provider
//            let d = IO.Path.GetDirectoryName(thisAssembly.FullName)
//            IO.Path.Combine(d, expectedName)
//        if IO.File.Exists expectedLocation then Assembly.LoadFrom expectedLocation else null
//        )
//    do System.AppDomain.CurrentDomain.add_AssemblyResolve handler

    let initFn (typeName : string) (args : obj []) = 
        let acProvidedType = ProvidedTypeDefinition(thisAssembly, namespaceName, typeName, baseType = Some typeof<obj>)
        acProvidedType.AddMember(ProvidedConstructor(parameters = [], InvokeCode = (fun args -> <@@ null @@>)))
        
        let getDbProperties () = 
            Database.listDbs (args.[0] :?> string) (args.[1]:?> string)
            |> List.map(fun d -> new ProvidedProperty(d.Name, typeof<ProvidedDatabaseType>, IsStatic = true, GetterCode = (fun _ -> <@@ d @@>)))
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