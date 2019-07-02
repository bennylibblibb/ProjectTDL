using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using GOSTS.Common;
using System.Data;
using System.Linq;
using System.ComponentModel;
using GOSTS.ViewModel;
using GOSTS;

namespace GOSTS.ViewModel
{ 
    public class InstmntViewModel : TreeViewItemViewModel
    {
        readonly Instmnt _region;

        public InstmntViewModel(Instmnt region)
            : base(null, true)
        {
            _region = region;
        }

        public string InstmntName
        {
            get { return _region.InstmntCode; }
        }

        protected override void LoadChildren()
        {
            foreach (ProdType prodType in Database.GetProdTypes(_region))
                base.Children.Add(new ProdTypeViewModel(prodType, this));
        }
    }

    public class ProdTypeViewModel : TreeViewItemViewModel
    {
        readonly ProdType _prodType;

        public ProdTypeViewModel(ProdType prodType, InstmntViewModel parentInstmnt)
            : base(parentInstmnt, true)
        {
            _prodType = prodType;
        }

        public string ProdTypeName
        {
            get { return _prodType.ProdTypeName; }
        }

        protected override void LoadChildren()
        {
            if (_prodType.ProdTypeCode != "2")
            {
                foreach (Prod prod in Database.GetProds(_prodType))
                    base.Children.Add(new ProdViewModel(prod, this));
            }
            else
            {
                foreach (ProdOption prodOption in Database.GetProdOptions(_prodType))
                    base.Children.Add(new ProdOptionViewModel(prodOption, this));
             }
        }
    }

    public class ProdViewModel : TreeViewItemViewModel
    {
        readonly Prod _prod;

        public ProdViewModel(Prod prod, ProdTypeViewModel parentProdType)
            : base(parentProdType, false)
        {
            _prod = prod;
        }

        public string ProdCode
        {
            get { return _prod.ProdCode; }
        }
        public string ProdName
        {
            get { return _prod.ProdName ; }
        }
    }

    public class ProdsViewModel : TreeViewItemViewModel
    {
        readonly Prod _prod;

        public ProdsViewModel(Prod prod, ProdOptionTypeViewModel parentProdOptionTypeViewModel)
            : base(parentProdOptionTypeViewModel, false)
        {
            _prod = prod;
        }

        public string ProdCode
        {
            get { return _prod.ProdCode; }
        }
        public string ProdName
        {
            get { return _prod.ProdName; }
        }
    }

    public class ProdOptionViewModel : TreeViewItemViewModel
    {
        readonly ProdOption  _prodOption;

        public ProdOptionViewModel(ProdOption prodOption, ProdTypeViewModel parentProdType)
            : base(parentProdType, true)
        {
            _prodOption = prodOption;
        }

        public string ProdOptionName
        {
            get { return  _prodOption.ProdOptionName.ToString("yyyy/MM") ; }
        }

        protected override void LoadChildren()
        {
            foreach (ProdOptionType prodOptionType in Database.GetProdOptionTypes (_prodOption))
                base.Children.Add(new ProdOptionTypeViewModel(prodOptionType, this));
        }
    }

    public class ProdOptionTypeViewModel : TreeViewItemViewModel
    {
        readonly ProdOptionType  _prodOptionType;

        public ProdOptionTypeViewModel(ProdOptionType prodOptionType, ProdOptionViewModel parentProdOption)
            : base(parentProdOption, true)
        {
            _prodOptionType = prodOptionType;
        }

        public string ProdOptionTypeName
        {
            get { return _prodOptionType.ProdOptionTypeName ; }
        }

        protected override void LoadChildren()
        {
            foreach (Prod prod in Database.GetProds(_prodOptionType))
                base.Children.Add(new ProdsViewModel(prod, this));
        }
    } 

    public class TreeViewItemViewModel : INotifyPropertyChanged
    {
        #region Data

        static readonly TreeViewItemViewModel DummyChild = new TreeViewItemViewModel();

        readonly ObservableCollection<TreeViewItemViewModel> _children;
        readonly TreeViewItemViewModel _parent;

        bool _isExpanded;
        bool _isSelected;

        #endregion // Data

        #region Constructors

        protected TreeViewItemViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
        {
            _parent = parent;

            _children = new ObservableCollection<TreeViewItemViewModel>();

            if (lazyLoadChildren)
                _children.Add(DummyChild);
        }

        // This is used to create the DummyChild instance.
        private TreeViewItemViewModel()
        {
        }

        #endregion // Constructors

        #region Presentation Members

        #region Children

        /// <summary>
        /// Returns the logical child items of this object.
        /// </summary>
        public ObservableCollection<TreeViewItemViewModel> Children
        {
            get { return _children; }
        }

        #endregion // Children

        #region HasLoadedChildren

        /// <summary>
        /// Returns true if this object's Children have not yet been populated.
        /// </summary>
        public bool HasDummyChild
        {
            get { return this.Children.Count == 1 && this.Children[0] == DummyChild; }
        }

        #endregion // HasLoadedChildren

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;

                // Lazy load the child items, if necessary.
                if (this.HasDummyChild)
                {
                    this.Children.Remove(DummyChild);
                    this.LoadChildren();
                }
            }
        }

        #endregion // IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                    //_isExpanded = true;
                    //this.OnPropertyChanged("IsExpanded");
                }

                //// Expand all the way up to the root.
                //if (_isExpanded && _parent != null)
                //    _parent.IsExpanded = true;

                //// Lazy load the child items, if necessary.
                //if (this.HasDummyChild)
                //{
                //    this.Children.Remove(DummyChild);
                //    this.LoadChildren();
                //}

            }
        }

        #endregion // IsSelected

        #region LoadChildren

        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// Subclasses can override this to populate the Children collection.
        /// </summary>
        protected virtual void LoadChildren()
        {
        }

        #endregion // LoadChildren

        #region Parent

        public TreeViewItemViewModel Parent
        {
            get { return _parent; }
        }

        #endregion // Parent

        #endregion // Presentation Members

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members
    }
}
