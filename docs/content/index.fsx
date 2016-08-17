(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../bin"

(**
AzureDocumentDbTypeProvider
===========================

This library allows easy access to DocumentDb databases and collections through an F# Type Provider.

Installation
-------------

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
We now have intellisense to explore the databases in our DocumentDb account like so

<div class="row">
  <div class="span1"></div>
  <div class="span6">
    <div class="well well-small" id="nuget">
      <img src = "img/DbCapture.PNG">
    </div>
  </div>
  <div class="span1"></div>
</div>

We can get a handle on the SDK db object with:
*)

let catalogueDb = Tp.Databases.Catalogue.ReadDatabase

(**
and access our collections with:
*)

let partsCollection = Tp.Databases.Catalogue.Collections.Parts


(**

Documentation
-------------

The library comes with comprehensible documentation. 
It can include tutorials automatically generated from `*.fsx` files in [the content folder][content]. 
The API reference is automatically generated from Markdown comments in the library implementation.

 * [API Reference](reference/index.html) contains automatically generated documentation for all types, modules
   and functions in the library. This includes additional brief samples on using most of the
   functions.

RoadMap
--------------------------

This project is currently still pre-release. Planned features will generally be added to the [issues page][issues].

 
Contributing and copyright
--------------------------

The project is hosted on [GitHub][gh] where you can [report issues][issues], fork 
the project and submit pull requests. If you're adding a new public API, please also 
consider adding [samples][content] that can be turned into a documentation. You might
also want to read the [library design notes][readme] to understand how it works.

The library is available under MIT license, which allows modification and 
redistribution for both commercial and non-commercial purposes. For more information see the 
[License file][license] in the GitHub repository. 

  [content]: https://github.com/stewart-r/AzureDocumentDbTypeprovider/tree/master/docs/content
  [gh]: https://github.com/stewart-r/AzureDocumentDbTypeprovider
  [issues]: https://github.com/stewart-r/AzureDocumentDbTypeprovider/issues
  [readme]: https://github.com/stewart-r/AzureDocumentDbTypeprovider/blob/master/README.md
  [license]: https://github.com/stewart-r/AzureDocumentDbTypeprovider/blob/master/LICENSE.txt
*)
