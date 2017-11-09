# Microsoft Bot Builder Overview

Microsoft Bot Builder is a powerful framework for constructing bots that can handle both freeform interactions and more guided ones where the possibilities are explicitly shown to the user. This samples show that it is feasible to use and leverage F# to provide a natural way to write Bots.

Please note that these samples has been created with F# 4.1, Visual Studio 2017, and target .NET Framework 4.6, aside of that they try to follow in as much as possible the functionality and general code structure of the original C# samples.

High Level Features:
* Powerful dialog system with dialogs that are isolated and composable.  
* Built-in dialogs for simple things like Yes/No, strings, numbers, enumerations.  
* Built-in dialogs that utilize powerful AI frameworks like [LUIS](http://luis.ai)
* Bots are stateless which helps them scale.  
* Form Flow for automatically generating a Bot from a F# class for filling in the class and that supports help, navigation, clarification and confirmation.

[Get started with the Bot Builder!](http://docs.botframework.com/sdkreference/csharp/)

There are several samples in this directory.
* [Microsoft.Bot.FunSample.SimpleEchoBot](SimpleEchoBot/) -- Bot Connector example done with the Bot Builder framework.
* [Microsoft.Bot.FunSample.EchoBot](EchoBot/) -- Add state onto the previous example.
* [Microsoft.Bot.FunSample.SimpleSandwichBot](SimpleSandwichBot/) -- FormFlow example of how easy it is to create a rich dialog with guided conversation, help and clarification. 

There are no F# implementations for these already, hopefully as time goes by...
* [Microsoft.Bot.FunSample.AnnotatedSandwichBot](AnnotatedSandwichBot/) -- Builds on the previous example to add attributes, messages, confirmation and business logic.
* [Microsoft.Bot.FunSample.SimpleAlarmBot](SimpleAlarmBot/) -- Integration of http://luis.ai with the dialog system to set alarms.
* [Microsoft.Bot.FunSample.AlarmBot](AlarmBot/) -- Add alarm logic to previous bot and send alarms proactively
* [Microsoft.Bot.FunSample.PizzaBot](PizzaBot/) -- Integration of http://luis.ai with FormFlow.
* [Microsoft.Bot.FunSample.GraphBot](GraphBot/Microsoft.Bot.Sample.GraphBot) -- Integration of [Microsoft Graph Api](https://graph.microsoft.io) with dialog system.
* [Microsoft.Bot.FunSample.SimpleFacebookAuthBot](SimpleFacebookAuthBot/) -- A bot showcasing OAuth authentication using Facebook graph API.
* [Microsoft.Bot.FunSample.SimpleIVRBot](SimpleIVRBot/) -- A sample IVR bot using Skype calling API.
* [Stock_Bot](Stock_Bot/) -- Samples that show calling a web service, LUIS, and LUIS Dialog.
* [SearchPoweredBots](SearchPoweredBots) -- Samples that show integration of [Azure Search](https://azure.microsoft.com/en-us/services/search/) with dialogs.

**You can find more samples in the [Bot Builder SDK Samples repo](https://github.com/Microsoft/BotBuilder-Samples/tree/master/CSharp)**
