namespace Microsoft.Bot.FunSample.SimpleEchoBot

open Microsoft.Bot.Builder.Dialogs
open Microsoft.Bot.Connector
open System
open System.Net
open System.Net.Http
open System.Threading.Tasks
open System.Web.Http
//open System.Web.Http.Description

[<Serializable>]
type EchoDialog() =

    let rec messageReceivedAsync (context: IDialogContext) (argument: IAwaitable<IMessageActivity>) =
        async {
            let message = argument.GetAwaiter().GetResult()
            let length = if isNull message.Text then 0 else message.Text.Length
            do! sprintf "You sent %s which was %d characters" message.Text length 
                |> context.PostAsync |> Async.AwaitTask
            context.Wait messageReceivedAsync
        } |> Async.StartAsTask :> Task

    interface IDialog with
        member this.StartAsync context =
            context.Wait messageReceivedAsync
            Task.CompletedTask

[<BotAuthentication>]
type MessagesController() =
    inherit ApiController()

    let handleSystemMessage (message: Activity) =
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
            // Handle knowing that the user is typing
        | ActivityTypes.Ping -> ()
        | _ -> ()

        null :> Activity

    member this.Post ([<FromBody>]activity : Activity) =
        async {
            if not (isNull activity) && activity.Type = ActivityTypes.Message then
                do! Conversation.SendAsync (activity, fun _ -> upcast EchoDialog()) 
                    |> Async.AwaitTask
            else
                handleSystemMessage activity |> ignore
            return new HttpResponseMessage (HttpStatusCode.Accepted)
        } |> Async.StartAsTask
