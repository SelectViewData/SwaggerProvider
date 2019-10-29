module Swagger.Slack.Tests

open SwaggerProvider
open Expecto
open System

let [<Literal>] Schema = __SOURCE_DIRECTORY__ + "/../Schemas/v2/Slack.json"
let [<Literal>] Host = "https://slack.com"
type Slack = SwaggerClientProvider<Schema, PreferAsync = true>

//[<Tests>]
let slackTests =
  let slack = Slack.Client()
  slack.HttpClient.BaseAddress <- Uri Host
  
  testList "All/TP Slack Tests" [
    testCaseAsync "call provided methods" <| async {
        let! result =
          slack.ChatPostEphemeral(
            threadTs = None,
            blocks = "",
            attachments = "",
            asUser = None,
            parse = "",
            token = "",
            text = "Hello",
            user = "UNL9PF6P6",
            linkNames = None,
            channel = "CPDMKJR34") |> Async.Ignore
        Expect.equal true true "wahoo"
    }
  ]