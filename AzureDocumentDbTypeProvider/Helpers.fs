
[<AutoOpen>]
module ProviderImplementation.ProvidedTypes.ProvidedTypesHelpers

let inline makeProvidedConstructor parameters invokeCode =
  ProvidedConstructor(parameters, InvokeCode = invokeCode)

let inline makeProvidedProperty< ^T> getterCode propName isStatic =
  ProvidedProperty(propName, typeof< ^T>, GetterCode = getterCode, IsStatic = isStatic)

let inline makeProvidedMethod< ^T> parameters invokeCode methodName =
  ProvidedMethod(methodName, parameters, typeof< ^T>, InvokeCode = invokeCode)

let inline makeProvidedParameter< ^T> paramName =
  ProvidedParameter(paramName, typeof< ^T>)

let inline addXmlDocDelayed comment providedMember =
  (^a : (member AddXmlDocDelayed : (unit -> string) -> unit) providedMember, (fun () -> comment))
  providedMember

