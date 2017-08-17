using System;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace Actiance.Interface
{
    public interface IAppDialog<T> : IDialog<T>
    {
        Task TypeAndMessage(IDialogContext context, string response);
    }
}
