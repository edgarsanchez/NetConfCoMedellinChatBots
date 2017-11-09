namespace Microsoft.Bot.FunSample.EchoBot

open Microsoft.Bot.Builder.Dialogs
open Microsoft.Bot.Connector
open System
open System.Collections.Generic
open System.Threading.Tasks

[<Serializable>]
type EchoAttachmentDialog() =
    inherit EchoDialog()

    override this.MessageReceivedAsync context =
        // Get a handle for the base function implementation
        // We do it here and in this funny way because base calls can't be done in a closure
        // See https://stackoverflow.com/questions/5847202/base-values-may-only-be-used-to-make-direct-calls-to-the-base-implementations
        let baseMessageReceivedAsync = base.MessageReceivedAsync context
        fun argument ->
            async {
                let message = argument.GetAwaiter().GetResult()
                if message.Text.ToLower() = "makeattachment" then
                    let reply = context.MakeMessage(Text = sprintf "%d: Yoy said %s" this.count message.Text, 
                                                    AttachmentLayout = AttachmentLayoutTypes.Carousel )
                    this.count <- this.count + 1

                    let actions = List<CardAction>()
                    for i in 0 .. 2 do
                        CardAction(ActionTypes.ImBack, title = sprintf "Button:%d" i, value = sprintf "Action:%d" i )
                        |> actions.Add
                    for i in 0 .. 4 do
                        let imageList = List<CardImage>()
                        CardImage(Url = sprintf "https://placeholdit.imgix.net/~text?txtsize=35&txt=image%d&w=120&h=120" i)
                        |> imageList.Add
                        HeroCard(title = sprintf "title%d" i, images = imageList, buttons = actions).ToAttachment()
                        |> reply.Attachments.Add

                    do! reply |> context.PostAsync |> Async.AwaitTask
                    context.Wait this.MessageReceivedAsync
                else
                    do! baseMessageReceivedAsync argument |> Async.AwaitTask
            } |> Async.StartAsTask :> Task
