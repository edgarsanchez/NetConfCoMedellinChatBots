module Microsoft.Bot.FunSample.EchoBot.EchoChainDialog

open Microsoft.Bot.Builder.Dialogs
open System.Text.RegularExpressions

let dialog =
    Chain.PostToChain()
        .Select(fun msg -> msg.Text)
        .Switch(
            Case( (fun text -> Regex("^reset", RegexOptions.IgnoreCase).Match(text).Success),
                fun _ _ -> Chain.From(fun _ ->
                                upcast PromptDialog.PromptConfirm(
                                    "Are you sure you want to reset the count?", "Didn't get that", 3, PromptStyle.Keyboard ) )
                            .ContinueWith(fun ctx res -> 
                                async {
                                    return 
                                        if res.GetAwaiter().GetResult() then
                                            ctx.UserData.SetValue("count", 0)
                                            "Reset count."
                                        else
                                            "Did not reset count."
                                    |> Chain.Return } |> Async.StartAsTask ) ),
            RegexCase( Regex("^help", RegexOptions.IgnoreCase),
                fun _ _ -> "I am a simple echo dialog with a counter! Reset my counter by typing 'reset'!" |> Chain.Return ),
            DefaultCase(fun context txt ->
                let _, count = context.UserData.TryGetValue("count")
                let newCount = count + 1
                context.UserData.SetValue("count", newCount)
                sprintf "%d: You said %s" newCount txt |> Chain.Return ) )
        .Unwrap()
        .PostToUser()
 