namespace BasicMultiDialogBot

open BasicMultiDialogBot.Dialogs
open Microsoft.Bot.Builder.Dialogs
open Microsoft.Bot.Connector
open System
open System.Net
open System.Net.Http
open System.Threading.Tasks
open System.Web.Http
open System.Web.Http.Description

//[<Serializable>]
//type EchoDialog() =
//    let rec MessageReceivedAsync (context: IDialogContext) (argument: IAwaitable<IMessageActivity>) =
//        async {
//            let message = argument.GetAwaiter().GetResult()
//            do! "You said " + message.Text |> context.PostAsync |> Async.AwaitTask
//            context.Wait MessageReceivedAsync
//        } |> Async.StartAsTask :> Task

//    interface IDialog with
//        member this.StartAsync (context: IDialogContext) =
//            context.Wait MessageReceivedAsync
//            Task.CompletedTask

[<BotAuthentication>]
type MessagesController() =
    inherit ApiController()

    let HandleSystemMessage (message: Activity) =
        match message.Type with
        | ActivityTypes.DeleteUserData -> ()
            // Implement user deletion here
            // If we handle user deletion, return a real message
        | ActivityTypes.ConversationUpdate -> ()
            // Handle conversation state changes, like members being added and removed
            // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
            // Not available in all channels
        | ActivityTypes.ContactRelationUpdate -> ()
            // Handle add/remove from contact lists
            // Activity.From + Activity.Action represent what happened
        | ActivityTypes.Typing -> ()
            // Handle knowing tha the user is typing
        | ActivityTypes.Ping -> ()
        | _ -> ()

        null :> Activity

    [<ResponseType(typeof<unit>)>]
    member this.Post ([<FromBody>]activity : Activity) =
        async {
            if activity <> null && activity.Type = ActivityTypes.Message then
                do! Conversation.SendAsync(activity, fun _ -> upcast RootDialog()) |> Async.AwaitTask
            else
                HandleSystemMessage activity |> ignore
            return new HttpResponseMessage(HttpStatusCode.Accepted)
        } |> Async.StartAsTask
