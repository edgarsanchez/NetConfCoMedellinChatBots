namespace BasicMultiDialogBot.Dialogs

open Microsoft.Bot.Builder.Dialogs
open Microsoft.Bot.Connector
open System
open System.Threading.Tasks

[<Serializable>]
type NameDialog() =
    let mutable attemps = 3

    let rec MessageReceivedAsync (context: IDialogContext) (argument: IAwaitable<IMessageActivity>) =
        async {
            let message = argument.GetAwaiter().GetResult()
            if String.IsNullOrWhiteSpace message.Text then
                attemps <- attemps - 1
                if attemps > 0 then
                    do! "I'm sorry, I don't understand your reply. What is your name (e.g. 'Bill', 'Melinda')?"
                        |> context.PostAsync |> Async.AwaitTask
                    context.Wait MessageReceivedAsync
                else
                    context.Fail (TooManyAttemptsException ("Message was not a string or was an empty string."))
            else
                context.Done message.Text
        } |> Async.StartAsTask :> Task

    interface IDialog<string> with
        member this.StartAsync context =
            async {
                do! "What is your name?" |> context.PostAsync |> Async.AwaitTask
                context.Wait MessageReceivedAsync
            } |> Async.StartAsTask :> Task
