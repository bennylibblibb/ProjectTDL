using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using GOSTS.Common;
using System.Data ; 
using System.Drawing ;


namespace GOSTS.ViewModel
{
    public class TabControlViewModel : INotifyPropertyChanged
    {
        private TabPageViewModel selectedPage; 

        public TabControlViewModel()
        {  
            Pages = new ObservableCollection<TabPageViewModel>(); 
           // SelectedPage = Pages.First();
            AddPageCommand = new AddPageCommand(this);
            RemovePageCommand = new RemovePageCommand(this);
        }

        public TabControlViewModel(string tabType, MarketPriceData data)
        {
            if (data.MarketCodeList == null) return;

            Pages = new ObservableCollection<TabPageViewModel>();

            if (tabType == "Tree")
            {
                foreach (string str in data.MarketCodeList)
                {
                    Pages.Add(new MarketCodeViewModel() { Header = str, Instmnts = new MarketCodeViewModel().MarketCodeViewModels(Database.GetInstmnts(str, data.MarketTreeTable)) });
                    //Pages.Add(new MarketCodeViewModel(str,Database.GetInstmnts(str, data.MarketTreeTable)));
                }
            }
            else if (tabType == "Grid")
            {
                Pages.Add(new TabPageViewModel() { Header = "1", MarketPriceItems = null });
            }
            SelectedPage = Pages.First();
            AddPageCommand = new AddPageCommand(this);
            RemovePageCommand = new RemovePageCommand(this);
        }

        public ObservableCollection<TabPageViewModel> Pages { get; private set; }
    
        public TabPageViewModel SelectedPage
        {
            get { return selectedPage; }
            set
            {
                selectedPage = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedPage"));
            }
        }

        public ICommand AddPageCommand { get; private set; }

        public ICommand RemovePageCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class RemovePageCommand : ICommand
    {
        public TabControlViewModel ViewModel { get; private set; }

        public RemovePageCommand(TabControlViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public void Execute(object parameter)
        {
            if (ViewModel.Pages.Count > 1)
            {
                ViewModel.Pages.Remove(ViewModel.SelectedPage);
                //  ViewModel.Pages.RemoveAt(ViewModel.Pages.Count() - 1);
            }
        }

        public bool CanExecute(object parameter)
        {
            //if (parameter!=null&&!(bool)parameter)
            //    return false ;
            return (ViewModel.Pages.Count() >= 1);
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;
    }

    public class AddPageCommand : ICommand
    {
        public TabControlViewModel ViewModel { get; private set; }

        public AddPageCommand(TabControlViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public void Execute(object parameter)
        {
            TabPageViewModel addTab = new TabPageViewModel() { Header = (ViewModel.Pages.Count + 1).ToString() };
            ViewModel.Pages.Add(addTab);
            ViewModel.SelectedPage = addTab;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

    }
}
