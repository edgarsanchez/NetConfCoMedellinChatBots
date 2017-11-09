namespace Microsoft.Bot.FunSample.AnnotatedSandwichBot

open Microsoft.Bot.Builder.Dialogs
open Microsoft.Bot.Builder.FormFlow
open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks

// The SandwichOrder is the simple form you want to fill out.  It must be serializable so the bot can be stateless.
// The order of fields defines the default order in which questions will be asked.
// Enumerations shows the legal options for each field in the SandwichOrder and the order is the order values will be presented 
// in a conversation.

type SandwichOptions = 
    BLT=0 | BlackForestHam=1 | BuffaloChicken=2 | ChickenAndBaconRanchMelt=3 | ColdCutCombo=4
    | MeatballMarinara=5 | OvenRoastedChicken=6 | RoastBeef=7 
    | [<Terms ("@rotis\w* style chicken", MaxPhrase = 3)>] RotisserieStyleChicken=8 | SpicyItalian=9
    | SteakAndCheese=10 | SweetOnionTeriyaki=11 | Tuna=12 | TurkeyBreast=13 | Veggie=14
type LengthOptions = SixInch=0 | FootLong=1
type BreadOptions = 
    // Use an image if generating cards
    // | [<Describe(Image = @"https://placeholdit.imgix.net/~text?txtsize=12&txt=Special&w=100&h=40&txttrack=0&txtclr=000&txtfont=bold")>]
    NineGrainWheat=0 | NineGrainHoneyOat=1 | Italian=2 | ItalianHerbsAndCheese=3 | Flatbread=4
type CheeseOptions = American=0 | MontereyCheddar=1 | Pepperjack=2
type ToppingOptions =
    // This starts at 1 because 0 is the "no value" value
    | [<Terms("except", "but", "not", "no", "all", "everything")>] Everything = 1
    | Avocado=2 | BananaPeppers=3 | Cucumbers=4 | GreenBellPeppers=5 | Jalapenos=6
    | Lettuce=7 | Olives=8 | Pickles=9 | RedOnion=10 | Spinach=11 | Tomatoes=12

type SauceOptions =
    ChipotleSouthwest=1 | HoneyMustard=2 | LightMayonnaise=3 | RegularMayonnaise=4
    | Mustard=5 | Oil=6 | Pepper=7 | Ranch=8 | SweetOnion=9 | Vinegar=10

[<Serializable>]
[<Template(TemplateUsage.NotUnderstood, "I do not understand \"{0}\".", "Try again, I don't get \"{0}\".")>]
[<Template(TemplateUsage.EnumSelectOne, "What kind of {&} would you like on your sandwich? {||}")>]
// [<Template(TemplateUsage.EnumSelectOne, "What kind of {&} would you like on your sandwich? {||}", ChoiceStyle = ChoiceStyleOptions.PerLine)>]
type SandwichOrder() =
    [<Prompt("What kind of {&} would you like? {||}")>]
    [<Describe(Image = @"https://placeholdit.imgix.net/~text?txtsize=16&txt=Sandwich&w=125&h=40&txttrack=0&txtclr=000&txtfont=bold")>]
    // [<Prompt("What kind of {&} would you like? {||}", ChoiceFormat ="{1}")>]
    // [<Prompt("What kind of {&} would you like?")>]
    [<DefaultValue>] val mutable Sandwich : Nullable<SandwichOptions>

    [<Prompt("What size of sandwich do you want? {||}")>]
    [<DefaultValue>] val mutable Length : Nullable<LengthOptions>

    // Specify Title and SubTitle if generating cards
    [<Describe(Title = "Sandwich Bot", SubTitle = "Bread Picker")>]
    [<DefaultValue>] val mutable Bread : Nullable<BreadOptions>

    // An optional annotation means that it is possible to not make a choice in the field.
    [<Optional>]
    [<DefaultValue>] val mutable Cheese : Nullable<CheeseOptions>

    [<Optional>]
    [<DefaultValue>] val mutable Toppings : List<ToppingOptions>

    [<Optional>]
    [<DefaultValue>] val mutable Sauces : List<SauceOptions>

    [<Optional>]
    [<Template(TemplateUsage.NoPreference, "None")>]
    [<DefaultValue>] val mutable Specials : string

    [<DefaultValue>] val mutable DeliveryAddress : string

    [<Pattern(@"(\(\d{3}\))?\s*\d{3}(-|\s*)\d{4}")>]
    [<DefaultValue>] val mutable PhoneNumber : string

    [<Optional>]
    [<Template(TemplateUsage.StatusFormat, "{&}: {:t}", FieldCase = CaseNormalization.None)>]
    [<DefaultValue>] val mutable DeliveryTime : Nullable<DateTime>

    [<Numeric(1, 5)>]
    [<Optional>]
    [<Describe("your experience today")>]
    [<DefaultValue>] val mutable Rating : Nullable<float>


    static member BuildForm() =
        //let processOrder : OnCompletionAsyncDelegate<SandwichOrder> = fun (context : IDialogContext) _ ->
        //let processOrder : ValidateAsyncDelegate<IDialogContext> = fun (context : IDialogContext, _) ->
        let processOrder = fun (context : IDialogContext) _ ->
            async {
                do! "We are currently processing your sandwich. We will message you the status." 
                    |> context.PostAsync |> Async.AwaitTask
            } |> Async.StartAsTask :> Task

        FormBuilder<SandwichOrder>()
            .Message("Welcome to the simple sandwich order bot!")
            .Field("Sandwich")
            .Field("Length")
            .Field("Bread")
            .Field("Cheese")
            .Field("Toppings",
                validate = ( fun (state, value : List<obj>) -> async {
                    let values = (value :> List<obj>).OfType<ToppingOptions>()
                    let result = ValidateResult (IsValid = true)
                    result.Value <-
                        if not (isNull values) && (values.Contains ToppingOptions.Everything) then
                            ToppingOptions.GetValues
                            |> Array.filter (fun topping -> topping <> ToppingOptions.Everything && not (values.Contains topping))
                        else
                            values
                    return result }
                    |> Async.StartAsTask ) )

    