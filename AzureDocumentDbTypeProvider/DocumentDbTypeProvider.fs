namespace DocumentDbTypeProvider

open ProviderImplementation.ProvidedTypes
open Microsoft.FSharp.Core.CompilerServices
open System.Reflection

[<TypeProvider>]
type public DocumentDbTypeProvider(config: TypeProviderConfig) as this = 
    inherit TypeProviderForNamespaces()

    let nsName = "FSharp.Azure.DocumentDbTypeProvider"
    let thisAssembly = Assembly.GetExecutingAssembly()
    let docDbType = ProvidedTypeDefinition(thisAssembly,nsName,"DocumentDbTypeProvider", baseType = Some typeof<obj>)

    do
        this.AddNamespace(nsName,[docDbType])

[<TypeProviderAssembly>]
do ()