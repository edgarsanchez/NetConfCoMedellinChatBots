using Microsoft.Bot.Builder.Dialogs;
using Microsoft.FSharp.Core;
using System;
using System.Threading.Tasks;

namespace CSharpCompatibility
{
    public delegate IDialog<T> UnitToIDialog<T>(Unit x);

    public static class TypeCasting
    {

        public static Func<IDialog<object>> ToFuncIDialogObj<T>(UnitToIDialog<T> f) where T: class
        {
            return f as Func<IDialog<object>>;
        }

        public static IDialog<object> ToIDialogObj<T>(IDialog<T> dialog) where T: class
        {
            return dialog;
        }
    }
}
