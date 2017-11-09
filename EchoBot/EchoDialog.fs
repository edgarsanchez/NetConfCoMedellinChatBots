namespace Microsoft.Bot.FunSample.EchoBot

open Microsoft.Bot.Builder.Dialogs
open Microsoft.Bot.Connector
open System
open System.Threading.Tasks

[<Serializable>]
type EchoDialog() =
    member val count = 1 with get, set

    // Normally the type should be IDialogContext -> IAwaitable<IMessageActivity> -> Task
    // We are forcing right associativity so that in a descendant, e.g. EchoLocationDialog, we can capture the base function
    // See the Context.Wait call in the overridden MessageReceivedAsync there
    abstract member MessageReceivedAsync : IDialogContext -> (IAwaitable<IMessageActivity> -> Task)

    // In the end MessageReceivedAsync has indeed 2 parameters. We do it in this funny way
    // so that a descendant can capture this base definition with one argument, and call it later adding the second argument
    // See the Context.Wait call in the overridden MessageReceivedAsync in EchoLocationDialog
    // See https://stackoverflow.com/questions/5847202/base-values-may-only-be-used-to-make-direct-calls-to-the-base-implementations
    default this.MessageReceivedAsync context = fun argument ->
        async {
            let message = argument.GetAwaiter().GetResult()
            if message.Text = "reset" then
                PromptDialog.Confirm(context, ResumeAfter this.AfterResetAsync, 
                    "Are you sure you want to reset the count?", "Didn't get that!", promptStyle = PromptStyle.None)
            else
                do! sprintf "%d: You said %s" this.count message.Text |> context.PostAsync |> Async.AwaitTask
                this.count <- this.count + 1
                context.Wait this.MessageReceivedAsync
        } |> Async.StartAsTask :> Task

    member this.AfterResetAsync context argument =
        async {
            let confirm = argument.GetAwaiter().GetResult()
            let response = if confirm then this.count <- 1; "Reset count." else "Did not reset count."
            do! response |> context.PostAsync |> Async.AwaitTask
            context.Wait this.MessageReceivedAsync
        } |> Async.StartAsTask :> Task
   
    interface IDialog with
        member this.StartAsync context =
            context.Wait this.MessageReceivedAsync
            Task.CompletedTask
