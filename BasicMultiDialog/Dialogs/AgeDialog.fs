namespace BasicMultiDialogBot.Dialogs

open Microsoft.Bot.Builder.Dialogs
open Microsoft.Bot.Connector
open System
open System.Threading.Tasks

[<Serializable>]
type AgeDialog(name: string) =

    let mutable attemps = 3

    let rec MessageReceivedAsync (context: IDialogContext) (argument: IAwaitable<IMessageActivity>) =
        async {
            let message = argument.GetAwaiter().GetResult()
            match Int32.TryParse message.Text with
            | (false, age)
            | (true, age) when age <=0 ->
                attemps <- attemps - 1
                if attemps > 0 then
                    do! "I'm sorry, I don't understand your reply. What is your age (e.g. '42')?"
                        |> context.PostAsync |> Async.AwaitTask
                    context.Wait MessageReceivedAsync
                else
                    context.Fail (TooManyAttemptsException ("Message was not a valid age."))
            | (true, age) -> context.Done age
        } |> Async.StartAsTask :> Task

    member this.Name = name

    interface IDialog<int> with
        member this.StartAsync context =
            async {
                do! this.Name + ", what is your age?" |> context.PostAsync |> Async.AwaitTask
                context.Wait MessageReceivedAsync
            } |> Async.StartAsTask :> Task
