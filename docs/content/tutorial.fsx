(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../bin"

(**

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

