namespace Microsoft.Bot.FunSample.SimpleSandwichBot

open CSharpCompatibility
open Microsoft.Bot.Builder.Dialogs
open Microsoft.Bot.Connector
open System.Net
open System.Net.Http
open System.Web.Http
open System.Web.Http.Description
open Microsoft.Bot.Builder.FormFlow
open System.Diagnostics

[<BotAuthentication>]
type MessagesController() =
    inherit ApiController()

    let makeRootDialog() =
        Chain.From (fun _ -> upcast FormDialog.FromForm (BuildFormDelegate SandwichOrder.BuildForm))

    /// <summary>
    /// POST: api/Messages
    /// receive a message from a user and send replies
    /// </summary>
    /// <param name="activity"></param>
    [<ResponseType (typeof<unit>)>]
    member this.Post ([<FromBody>] activity : Activity) =
        async {
            if not (isNull activity) then
                // one of these will have an interface and process it
                match activity.GetActivityType() with
                | ActivityTypes.Message ->
                    do! Conversation.SendAsync (activity, fun _ -> makeRootDialog() |> TypeCasting.ToIDialogObj) |> Async.AwaitTask
                | act ->
                    "Unknown activity type ignored: " + act |> Trace.TraceError 
            return new HttpResponseMessage(HttpStatusCode.Accepted)
        } |> Async.StartAsTask
