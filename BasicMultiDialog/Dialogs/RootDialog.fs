namespace BasicMultiDialogBot.Dialogs

open Microsoft.Bot.Builder.Dialogs
open Microsoft.Bot.Connector
open System
open System.Threading.Tasks

[<Serializable>]
type RootDialog() =
    let mutable name = ""

    let postTextAndWait (context: IDialogContext) (text: string) =
        text |> context.PostAsync |> Async.AwaitTask

    let rec ageDialogResumeAfter (context: IDialogContext) (result: IAwaitable<int>) =
        async {
            try
                let age = result.GetAwaiter().GetResult()
                do! sprintf "Your name is %s and your age is %d." name age
                    |> postTextAndWait context
                do! sendWelcomeMessageAsync context |> Async.AwaitTask
            with
            | :? TooManyAttemptsException ->
                do! "I'm sorry, I'm having issues understanding you. Let's try again."
                    |> postTextAndWait context
                do! sendWelcomeMessageAsync context |> Async.AwaitTask
        } |> Async.StartAsTask :> Task

    and nameDialogResumeAfter (context: IDialogContext) (result: IAwaitable<string>) =
        async {
            try
                name <- result.GetAwaiter().GetResult()
                context.Call (AgeDialog(name), ResumeAfter(ageDialogResumeAfter))
            with
            | :? TooManyAttemptsException ->
                do! "I'm sorry, I'm having issues understanding you. Let's try again." |> postTextAndWait context
                do! sendWelcomeMessageAsync context |> Async.AwaitTask
        } |> Async.StartAsTask :> Task
    
    and sendWelcomeMessageAsync (context: IDialogContext) =
        async {
            do! "Hi, I'm the Basic Multi Dialog bot. Let's get started." |> context.PostAsync |> Async.AwaitTask
            context.Call (NameDialog(), ResumeAfter(nameDialogResumeAfter))
        } |> Async.StartAsTask

    let rec MessageReceivedAsync (context: IDialogContext) (argument: IAwaitable<IMessageActivity>) =
        async {
            let message = argument.GetAwaiter().GetResult()
            do! sendWelcomeMessageAsync context |> Async.AwaitTask
        } |> Async.StartAsTask :> Task

    interface IDialog with
        member this.StartAsync context =
            context.Wait MessageReceivedAsync
            Task.CompletedTask
