namespace Microsoft.Bot.FunSample.EchoBot

open Microsoft.Bot.Builder.ConnectorEx
open Microsoft.Bot.Builder.Dialogs
open Microsoft.Bot.Connector
open Newtonsoft.Json.Linq
open System
open System.Collections.Generic
open System.Linq
open System.Threading
open System.Threading.Tasks

[<Serializable>]
type FacebookLocationDialog() =
    let LocationReceivedAsync (context : IDialogContext) (argument: IAwaitable<IMessageActivity>) =
        async {
            let msg = argument.GetAwaiter().GetResult()
            let location =
                if isNull msg.Entities then null
                else msg.Entities.Where(fun t -> t.Type = "Place").Select(fun t -> t.GetAs<Place>()).FirstOrDefault()
            context.Done location
        } |> Async.StartAsTask :> Task
            
    let MessageReceivedAsync (context : IDialogContext) (argument: IAwaitable<IMessageActivity>) =
        async {
            let msg = argument.GetAwaiter().GetResult()
            if msg.ChannelId = "facebook" then
                let fbReplies = List<FacebookQuickReply>()
                // If content_type is location, title and payload are not used
                // see https://developers.facebook.com/docs/messenger-platform/send-api-reference/quick-replies#fields
                // for more information.
                FacebookQuickReply(contentType = FacebookQuickReply.ContentTypes.Location, title = null, payload = null) |> fbReplies.Add
                let reply = context.MakeMessage(ChannelData = FacebookMessage(text = "Please share your location with me.", quickReplies = fbReplies))
                do! reply |> context.PostAsync |> Async.AwaitTask
                context.Wait LocationReceivedAsync
            else
                context.Done null
        } |> Async.StartAsTask :> Task

    interface IDialog<Place> with
        member this.StartAsync context =
            context.Wait MessageReceivedAsync
            Task.CompletedTask

[<Serializable>]
type EchoLocationDialog() =
    inherit EchoDialog()

    override this.MessageReceivedAsync context =
        // Get a handle for the base function implementation
        // We do it here and in this funny way because base calls can't be done in a closure
        // See https://stackoverflow.com/questions/5847202/base-values-may-only-be-used-to-make-direct-calls-to-the-base-implementations
        let baseMessageReceivedAsync = base.MessageReceivedAsync context
        fun argument ->
            async {
                let msg = argument.GetAwaiter().GetResult()
                if msg.Text.ToLower() = "location" then
                    do! context.Forward(FacebookLocationDialog(), this.ResumeAfter, msg, CancellationToken.None) |> Async.AwaitTask
                else
                    // add the second parameter to base.MessageReceiveAsync, call it, and wait until it finishes
                    do! baseMessageReceivedAsync argument |> Async.AwaitTask
            } |> Async.StartAsTask :> Task

    member this.ResumeAfter (context: IDialogContext) (result: IAwaitable<Place>) =
        let toMapUrl (geo : GeoCoordinates) =
            let lat = if geo.Latitude.HasValue then geo.Latitude.Value else 0.0
            let long = if geo.Longitude.HasValue then geo.Longitude.Value else 0.0
            sprintf "https://www.bing.com/maps/?v=2&cp=%f~%f&lvl=16&dir=0&sty=c&sp=point.%f_%f_You are here&ignoreoptin=1" lat long lat long
        async {
            let place = result.GetAwaiter().GetResult()
            if not (isNull place) then
                try
                    let geo = (place.Geo :?> JObject).ToObject<GeoCoordinates>()
                    let cardList = List<CardAction>()
                    CardAction(ActionTypes.OpenUrl, title = "Your location", value  = toMapUrl geo) |> cardList.Add
                    let reply = context.MakeMessage()
                    HeroCard(title = "Open your location in bing maps!", buttons = cardList).ToAttachment() |> reply.Attachments.Add 
                    do! reply |> context.PostAsync |> Async.AwaitTask
                with
                | _ ->
                    do! "No GeoCoordinates!" |> context.PostAsync |> Async.AwaitTask
            else
                do! "No localtion extracted!" |> context.PostAsync |> Async.AwaitTask
            context.Wait this.MessageReceivedAsync
        } |> Async.StartAsTask :> Task

    interface IDialog with
        member this.StartAsync context =
            context.Wait this.MessageReceivedAsync
            Task.CompletedTask
