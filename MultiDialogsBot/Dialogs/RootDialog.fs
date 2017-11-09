namespace MultiDialogsBot.Dialogs

open Microsoft.Bot.Builder.Dialogs
open Microsoft.Bot.Connector
open System
open System.Threading.Tasks

[<Serializable>]
type RootDialog() =

    interface IDialog with
    member this.StartAsync context =
        context.Wait this.MessageReceivedAsync
        Task.CompletedTask

