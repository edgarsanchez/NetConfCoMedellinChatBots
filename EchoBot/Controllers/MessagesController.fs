namespace Microsoft.Bot.FunSample.EchoBot

open Autofac
open CSharpCompatibility
open Microsoft.Bot.Builder.Dialogs
open Microsoft.Bot.Builder.Dialogs.Internals
open Microsoft.Bot.Builder.Scorables
open Microsoft.Bot.Connector
open System.Diagnostics
open System.Net
open System.Net.Http
open System.Text.RegularExpressions
open System.Threading.Tasks
open System.Web.Http
open System.Web.Http.Description


[<BotAuthentication>]
type MessagesController() =
    inherit ApiController()

    static do
        Conversation.UpdateContainer(fun builder ->
            let scorable = Actions
                            .Bind(fun (botToUser : IBotToUser) (_ : IMessageActivity) -> botToUser.PostAsync "polo")
                            .When(Regex("marco")).Normalize()
            builder.RegisterInstance(scorable).AsImplementedInterfaces().SingleInstance() |> ignore )

    /// <summary>
    /// POST: api/Messages
    /// receive a message from a user and send replies
    /// </summary>
    /// <param name="activity"></param>
    //[<ResponseType(typeof<unit>)>]
    member this.Post ([<FromBody>]activity : Activity) =
        async {
            if not (isNull activity) then
                // one of these will have an interface and process it
                match activity.GetActivityType() with
                | ActivityTypes.Message ->
                    //do! Conversation.SendAsync(activity, fun _ -> upcast EchoDialog()) |> Async.AwaitTask
                    //do! Conversation.SendAsync(activity, fun _ -> upcast EchoCommandDialog.dialog) |> Async.AwaitTask
                    do! Conversation.SendAsync(activity, fun _ -> upcast EchoAttachmentDialog()) |> Async.AwaitTask
                    //do! Conversation.SendAsync(activity, fun _ -> TypeCasting.ToIDialogObj EchoChainDialog.dialog) |> Async.AwaitTask
                | ActivityTypes.ConversationUpdate ->
                    let update = activity :> IConversationUpdateActivity
                    if update.MembersAdded.Count > 0 then
                        use scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity)
                        let client = scope.Resolve<IConnectorClient>()
                        let reply = activity.CreateReply()
                        for newMember in update.MembersAdded do
                            reply.Text <- "Welcome " +
                                if newMember.Id <> activity.Recipient.Id then
                                    newMember.Name
                                 else
                                    activity.From.Name
                            do! client.Conversations.ReplyToActivityAsync reply :> Task |> Async.AwaitTask
                | x ->
                    "Unknown activity type ignored: " + x |> Trace.TraceError

            return new HttpResponseMessage(HttpStatusCode.Accepted)
        } |> Async.StartAsTask
