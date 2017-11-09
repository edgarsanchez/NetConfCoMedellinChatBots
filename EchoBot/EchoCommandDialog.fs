module Microsoft.Bot.FunSample.EchoBot.EchoCommandDialog

open Microsoft.Bot.Builder.Dialogs
open Microsoft.Bot.Connector
open System.Text.RegularExpressions
open System.Threading.Tasks

// In PromptDialog.Confirm() and IDialogContext.Wait() they expect a two argument function
// But .ResultHandler and .MessageReceived expect just one tuple as argument
// This little helper function solves the gap
let apply f ctx res = f(ctx, res)

let dialog = 
    // Silly trick so that we can call dialog.ResultHandler and dialog.MessageReceived
    // in the middle of its definition (like it happens in .On() and .OnDefault() calls)
    // F# doesn't allow to use recursively a binding while it's being defined
    // so we first instantiate the object and then implicitly modify it with the .On() and .OnDefault() calls
    let dialog' = CommandDialog<obj>()
    dialog'.On(Regex("^reset", RegexOptions.IgnoreCase),
            (fun context _ -> 
                async { 
                    PromptDialog.Confirm(context, apply dialog'.ResultHandler, "Are you sure you want to reset the count?", "Didn't get that" )
                } |> Async.StartAsTask :> Task ),
            (fun context result ->
                async {
                    let response = 
                        if result.GetAwaiter().GetResult() then
                            context.UserData.SetValue("count", 0)
                            "Reset count."
                        else
                            "Did not reset count."
                    do! response |> context.PostAsync |> Async.AwaitTask } |> Async.StartAsTask :> Task ) )
        .OnDefault(fun context msg ->
            async {
                let message = msg.GetAwaiter().GetResult()
                let _, count = context.UserData.TryGetValue("count")
                let newCount = count + 1
                context.UserData.SetValue("count", newCount)
                do! sprintf "%d: You said %s" newCount message.Text |> context.PostAsync |> Async.AwaitTask 
                apply dialog'.MessageReceived |> context.Wait  } |> Async.StartAsTask :> Task )
 