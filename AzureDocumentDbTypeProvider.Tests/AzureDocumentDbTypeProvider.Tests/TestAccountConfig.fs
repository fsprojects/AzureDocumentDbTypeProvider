module TestAccountConfig
open Microsoft.Azure.Documents.Client
open System

//As there is no local test emulator available for documentdb we must perform integration tests against a live account
//
//Enter the details below:
[<Literal>]
let AccountEndpointUri = """{Insert your test account endpoint uri here}"""
[<Literal>]
let AccountKey = """{Insert your test account key here}"""


let validateTestAccountCredentials () = 
    if AccountEndpointUri = """{Insert your test account endpoint uri here}""" || AccountKey = """{Insert your test account key here}"""
    then failwith "No test account details have been specified. You must provide credentials for a DocumentDb account within the TestAccountConfig.fs file in order to be able to run the integration tests"
        
    let client = new DocumentClient(new Uri(AccountEndpointUri), AccountKey )
    client.OpenAsync().Wait()
