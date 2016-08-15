(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../bin"

(**
AzureDocumentDbTypeProvider
======================

Documentation

<div class="row">
  <div class="span1"></div>
  <div class="span6">
    <div class="well well-small" id="nuget">
      The AzureDocumentDbTypeProvider library can be <a href="https://nuget.org/packages/AzureDocumentDbTypeProvider">installed from NuGet</a>:
      <pre>PM> Install-Package AzureDocumentDbTypeprovider</pre>
    </div>
  </div>
  <div class="span1"></div>
</div>

Example
-------

We initialise the type provider using the account Uri and account key for an Azure DocumentDb account as follows:

*)
#r "AzureDocumentDbTypeProvider.dll"
open FSharp.Azure.DocumentDbTypeProvider

type Tp = DocumentDbTypeProvider<"https://mydbaccount.documents.azure.com:443/","TheAccountKeyFromTheAzurePortal==">

(**
we can then get intellisense for the databases in our account like so:
![alt text](img/DbCapture.PNG "DocumentDb intellisense list")

We can get a handle on the SDK db object with:
*)

let catDbObj = Tp.Databases.Catalogue.ReadDatabase


(**

Samples & documentation
-----------------------

The library comes with comprehensible documentation. 
It can include tutorials automatically generated from `*.fsx` files in [the content folder][content]. 
The API reference is automatically generated from Markdown comments in the library implementation.

 * [Tutorial](tutorial.html) contains a further explanation of this sample library.

 * [API Reference](reference/index.html) contains automatically generated documentation for all types, modules
   and functions in the library. This includes additional brief samples on using most of the
   functions.
 
Contributing and copyright
--------------------------

The project is hosted on [GitHub][gh] where you can [report issues][issues], fork 
the project and submit pull requests. If you're adding a new public API, please also 
consider adding [samples][content] that can be turned into a documentation. You might
also want to read the [library design notes][readme] to understand how it works.

The library is available under Public Domain license, which allows modification and 
redistribution for both commercial and non-commercial purposes. For more information see the 
[License file][license] in the GitHub repository. 

  [content]: https://github.com/fsprojects/AzureDocumentDbTypeprovider/tree/master/docs/content
  [gh]: https://github.com/fsprojects/AzureDocumentDbTypeprovider
  [issues]: https://github.com/fsprojects/AzureDocumentDbTypeprovider/issues
  [readme]: https://github.com/fsprojects/AzureDocumentDbTypeprovider/blob/master/README.md
  [license]: https://github.com/fsprojects/AzureDocumentDbTypeprovider/blob/master/LICENSE.txt
*)
