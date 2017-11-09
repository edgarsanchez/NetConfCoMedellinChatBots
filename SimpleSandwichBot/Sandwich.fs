namespace Microsoft.Bot.FunSample.SimpleSandwichBot

open Microsoft.Bot.Builder.FormFlow
open System
open System.Collections.Generic

// The SandwichOrder is the simple form you want to fill out.  It must be serializable so the bot can be stateless.
// The order of fields defines the default order in which questions will be asked.
// Enumerations shows the legal options for each field in the SandwichOrder and the order is the order values will be presented 
// in a conversation.

type SandwichOptions = 
    BLT=0 | BlackForestHam=1 | BuffaloChicken=2 | ChickenAndBaconRanchMelt=3 | ColdCutCombo=4
    | MeatballMarinara=5 | OvenRoastedChicken=6 | RoastBeef=7 | RotisserieStyleChicken=8 | SpicyItalian=9
    | SteakAndCheese=10 | SweetOnionTeriyaki=11 | Tuna=12 | TurkeyBreast=13 | Veggie=14
type LengthOptions = SixInch=0 | FootLong=1
type BreadOptions = NineGrainWheat=0 | NineGrainHoneyOat=1 | Italian=2 | ItalianHerbsAndCheese=3 | Flatbread=4
type CheeseOptions = American=0 | MontereyCheddar=1 | Pepperjack=2
type ToppingOptions =
    Avocado=1 | BananaPeppers=2 | Cucumbers=3 | GreenBellPeppers=4 | Jalapenos=5
    | Lettuce=6 | Olives=7 | Pickles=8 | RedOnion=9 | Spinach=10 | Tomatoes=11

type SauceOptions =
    ChipotleSouthwest=1 | HoneyMustard=2 | LightMayonnaise=3 | RegularMayonnaise=4
    | Mustard=5 | Oil=6 | Pepper=7 | Ranch=8 | SweetOnion=9 | Vinegar=10

[<Serializable>]
type SandwichOrder() =
    [<DefaultValue>] val mutable Sandwich : Nullable<SandwichOptions>
    [<DefaultValue>] val mutable Length : Nullable<LengthOptions>
    [<DefaultValue>] val mutable Bread : Nullable<BreadOptions>
    [<DefaultValue>] val mutable Cheese : Nullable<CheeseOptions>
    [<DefaultValue>] val mutable Toppings : List<ToppingOptions>
    [<DefaultValue>] val mutable Sauce : List<SauceOptions>

    static member BuildForm() =
        FormBuilder<SandwichOrder>().Message("Welcome to the simple sandwich order bot!").Build()
    