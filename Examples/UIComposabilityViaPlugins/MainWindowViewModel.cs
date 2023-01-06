using System.Windows.Input;
using Codefarts.WPFCommon.Commands;

namespace UIComposabilityViaPlugins;

public class MainWindowViewModel
{
    public MainWindowViewModel()
    {
    }

    public ICommand LoadPlugins
    {
        get
        {
            return new DelegateCommand(x => true,
                                       x =>
                                       {
                                           // code here
                                       });
        }
    }
}