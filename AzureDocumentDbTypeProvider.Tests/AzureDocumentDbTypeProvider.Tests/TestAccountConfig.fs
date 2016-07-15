module TestAccountConfig
open Microsoft.Azure.Documents.Client
open System

//As there is no local test emulator available for documentdb we must perform integration tests against a live account
//
//Enter the details below:
let accountEndpointUri = """{Insert your test account endpoint uri here}"""
let accountKey = """{Insert your test account key here}"""


let validateTestAccountCredentials () = 
    if 
        accountEndpointUri = """{Insert your test account endpoint uri here}""" || accountKey = """{Insert your test account key here}"""
    then
        failwith "No test account details have been specified. You must provide credentials for a DocumentDb account within the TestAccountConfig.fs file in order to be able to run the integration tests"
        
    let client = new DocumentClient(new Uri(accountEndpointUri), accountKey )
    client.OpenAsync().Wait()
