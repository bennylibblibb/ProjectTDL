using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Globalization;
using WPF.MDI;

namespace GOSTS.Accounts
{   
    public interface INumUpDown 
    {
        void Up();
        void Down();
    }
    /// <summary>
    /// ucTBNumber.xaml 的交互逻辑
    /// modidy 2013-08-29 去掉上下限检测，去掉keyup检测方法,在点提交按钮检测
    /// </summary>
    public partial class ucTBNumber : UserControl
    {
        DecimalTextBz decTbCheck = new DecimalTextBz();
        public ucTBNumber()
        {
            InitializeComponent();
            BindTextEvent();
         
        }

        //public static readonly DependencyProperty UpBoundProperty = DependencyProperty.Register("UpBound", typeof(decimal), typeof(ucTBNumber));
        //public decimal UpBound
        //{
        //    get { return (decimal)GetValue(UpBoundProperty); }
        //    set { SetValue(UpBoundProperty, value); }
        //}

        //public static readonly DependencyProperty DownBoundProperty = DependencyProperty.Register("DownBound", typeof(decimal), typeof(ucTBNumber));
        //public decimal DownBound
        //{
        //    get { return (decimal)GetValue(DownBoundProperty); }
        //    set { SetValue(DownBoundProperty, value); }
        //}

        #region properties
        private decimal _UpBound;
        public decimal UpBound
        {
            get{
                //return _UpBound;
                return AppFlag.PriceUpBound;
            }
            set{_UpBound=value;}
        }

        private decimal _DownBound;
        public decimal DownBound
        {
            get { 
               // return _DownBound;
                return AppFlag.PriceDownBound;
            }
            set { _DownBound = value; }
        }

        private decimal _step = 1;
        public decimal Step
        {
            get
            {
                if (ProdCode == "")
                {
                    return 1;
                }
                return _step;
            }
            set { _step = value; }
        }

        private string _ProdCode="";
        public string ProdCode
        {
            get { return _ProdCode; }
            set { _ProdCode = value; }
        }

        private int _decLen = 0;
        public int decLen
        {
            get { return _decLen; }//if prodcode 's dec length is change in real time,change to:
            set { _decLen = value; }
        }

        public string Text
        {
            get {
                return this.txtAmount.Text;
            }
            set {
                this.txtAmount.Text = value;
            }
        }

       public bool? ChkAO = false;
        #endregion

        #region IUpDown Implementation
        public void Up()
        {
            Increase(Step);
        }
        private void Increase(decimal _step)
        {
            string strText = this.txtAmount.Text.Trim();
            decimal I = 0;
            try
            {
                I = Convert.ToDecimal(strText);
            }
            catch { return; }
            I += _step;
           // I = VerifyPriceBound(I);
            this.txtAmount.Text = I.ToString();
            this.txtAmount.SelectionStart = this.txtAmount.Text.Length;
        }
        public void Down()
        {
            decrease(Step);
        }

        private void decrease(decimal _step)
        {
            string strText = this.txtAmount.Text.Trim();
            decimal I = 0;
            try
            {
                I = Convert.ToDecimal(strText);
            }
            catch { return; }

            I -= _step;
          //  I = VerifyPriceBound(I);
            this.txtAmount.Text = I.ToString();
            this.txtAmount.SelectionStart = this.txtAmount.Text.Length;
        }
        #endregion

        #region textbox Event Handler

        void BindTextEvent()
        {
            try
            {
                this.txtAmount.PreviewKeyDown += txtAmount_PreviewKeyDown;
                //  this.txtAmount.KeyUp += txtAmount_KeyUp;

                //  DataObject.AddPastingHandler(txtAmount, tbDecimal_Pasting);

                if (GOSTradeStation.customizeData.AlertData.isEnableWheel)
                {
                    for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
                    {
                        if (App.Current.Windows[intCounter] is GOSTradeStation)
                        {
                            App.Current.Windows[intCounter].PreviewMouseWheel += txtAmount_MouseWheel;
                            this.txtAmount.PreviewMouseDown += txtAmount_PreviewMouseDown;
                            //  App.Current.Windows[intCounter].MouseWheel += txtAmount_MouseWheel;
                            //Mouse.AddMouseWheelHandler(App.Current.Windows[intCounter], txtAmount_MouseWheel);
                            // GOSTradeStation s = App.Current.Windows[intCounter] as ; 
                        }
                    }
                }
            }
            catch (Exception ex) { }      
        }

        void txtAmount_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MdiContainer Container = App.Current.MainWindow.FindName("Container") as MdiContainer;
            if (mdiChild != null && Container != null)
            {
                if (mdiChild != Container.ActiveMdiChild)
                {
                    mdiChild.Focus();
                }
            }
            //if (mdiChild != null)
            //{
            //    if(mdiChild.
            //}
        }

        //void txtAmount_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        //{
           
        //    if (LastInput != null)
        //    {
        //        if(InputLanguageManager.Current.CurrentInputLanguage!=LastInput)
        //        InputLanguageManager.Current.CurrentInputLanguage = LastInput;
        //    }
        //}

        //CultureInfo LastInput;
        //string DefaultInputLanguage = "en-US";
        //void txtAmount_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        //{           
        //    LastInput = InputLanguageManager.Current.CurrentInputLanguage;
        //    if (LastInput.Name.StartsWith(DefaultInputLanguage))
        //        return;
        //    foreach (var lang in InputLanguageManager.Current.AvailableInputLanguages)
        //    {
        //        var langCultureInfo = lang as CultureInfo;
        //        if (langCultureInfo.Name.StartsWith(DefaultInputLanguage))
        //        {
        //            InputLanguageManager.Current.CurrentInputLanguage = langCultureInfo;
        //            break;
        //        }
        //    }
        //}

        //void ucTBNumber_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

      
        //protected override void OnMouseWheel(MouseWheelEventArgs e)
        //{
        //    IInputElement ele=FocusManager.GetFocusedElement(GOSTradeStation
        //    base.OnMouseWheel(e);
        //}

        public decimal get_txt_Price_NT()
        {
            if (ChkAO == true)
            {
                return 0;
            }
            try
            {
                return convertPrice(this.txtAmount.Text.Trim()); //  Convert.ToInt32(this.txt_Price_NT.Text.Trim());
            }
            catch
            {
                return 0;
            }
            return 0;
        }

        decimal convertPrice(string strPrice)
        {
            if (strPrice.EndsWith("."))
            {
                strPrice += "0";
            }
            return Convert.ToDecimal(strPrice);
        }


        public MdiChild mdiChild;
        public void SetMdiChild(MdiChild _mdiChild)
        {
            mdiChild = _mdiChild;
        }


        void txtAmount_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            MdiContainer Container = App.Current.MainWindow.FindName("Container") as MdiContainer;
            if (mdiChild != null && Container != null)
            {
                if (mdiChild != Container.ActiveMdiChild)
                {
                    //if (!DecimalTextBz.ExceptActiveMdiChild())
                    //{
                        return;
                    //}
                }
            }
           
           // this.txtAmount

            if (!this.txtAmount.IsKeyboardFocused)
            {
                return;
            }
            int i = e.Delta;
            int time = i / 100;
            if (ChkAO == true)
            {
                return;
            }
            Increase(time*Step);
            e.Handled = true;
        }

        void txtAmount_KeyUp(object sender, KeyEventArgs e)
        {
            string str = this.txtAmount.Text.Trim();
            int i = this.txtAmount.SelectionStart;
            decimal decNum = 0.0M;
            if (str == "")
            {
                return;
            }
            if (str == "-")
            {
                return;
            }
            try
            {
                decNum = Convert.ToDecimal(str);
            }
            catch
            {
                //input not a numeric character
                bool isNagative = false;
                if (str.StartsWith("-"))
                {
                    isNagative = true;
                    str = str.Substring(1);
                    if (i > str.Length)
                    {
                        i = str.Length;
                    }
                }

                int curPo = 0;
                if (i > 0)
                {
                    string strToCursor = str.Substring(0, i);
                    strToCursor = System.Text.RegularExpressions.Regex.Replace(strToCursor, @"[^\d\.]", "");
                    curPo = strToCursor.Length;
                }
                str = System.Text.RegularExpressions.Regex.Replace(str, @"[^\d\.]", "");
                decNum = Utility.ConvertToDecimal(str);
                if (isNagative)
                {
                    decNum = -decNum;
                    curPo += 1;
                }                  
                i = curPo;
            }
            //check if input over up bound and down bound
            decNum=this.VerifyPriceBound(decNum);
            str = decNum.ToString();       
            
            //check if dec length of input more than server specified 
            int l = this.decLen;

            int pos = str.IndexOf('.');
            if (pos > 0)
            {
                int decLen = str.Length - pos - 1;
                if (decLen > l)
                {
                    str = str.Substring(0, str.Length - (decLen - l));
                    if (l < 1)
                    {
                        str = str.Replace(".", "");
                    }                   
                }
            }
            this.txtAmount.Text = str;
            if (i > str.Length)
            {
                i = str.Length;
            }
            this.txtAmount.SelectionStart = i;
        }

        void txtAmount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.OemMinus || e.Key == Key.Subtract || e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Enter || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {}
            else
            {
                string strCode = e.Key.ToString();
                if (e.Key == Key.ImeProcessed)
                {
                    Key k = e.ImeProcessedKey;
                    strCode = k.ToString();                    
                }
              
                if (e.Key != Key.OemPeriod&&e.Key!=Key.Decimal)
                    if (!System.Text.RegularExpressions.Regex.IsMatch(strCode, @"(\d|\.|\-)"))//  isNumberic(e.Key.ToString()))
                    {
                        e.Handled = true;
                    }
                string sourceStr = this.txtAmount.Text.Trim();
                
            }
        }

        public string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        private void tbDecimal_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!System.Text.RegularExpressions.Regex.IsMatch(text, @"^[\-]{0,1}\d+(\.\d+)*$"))
                { e.CancelCommand(); }
            }
            else { e.CancelCommand(); }
        }
        #endregion

        public void SetProdCode(string _prodCode)
        {
            //set prodcode , and relative decimal length, step
            if (_prodCode == null) return;
            if (_prodCode.Trim() == this.ProdCode)
            {
                return;
            }
            this.ProdCode = _prodCode;
            this.decLen = GosBzTool.getDecLen(ProdCode);//if decimal length changes real time,put this to get property of decLen;
            if (decLen < 0)
            {
                decLen = 0;
            }
            if (decLen > 0)
            {
                decimal dec = Convert.ToDecimal(Math.Pow(10, decLen));
                dec = 1 / dec;
                this.Step = dec;
            }
            else
            {
                this.Step = 1;
            }
        }


        public decimal VerifyPriceBound(decimal d)
        {
            if (d > this.UpBound)
            {
                d = this.UpBound;
            }
            if (d <this.DownBound)
            {
                d = this.DownBound;
            }
            return d;
        }

        public bool  CheckPriceBound(decimal d)
        {
            if (d > this.UpBound)
            {
                return false;
            }
            if (d < this.DownBound)
            {
                return false;
            }
            return true;
        }

        public decimal GetInputPrice(bool? bAO)
        {
            if (bAO??false) return 0;
            string strPrice = this.txtAmount.Text.Trim();
            if (strPrice.EndsWith("."))
            {
                strPrice += "0";
            }
            decimal Dec=0M;
            try
            {
                Dec=Convert.ToDecimal(strPrice);
            }
            catch{
                
            }
            return Dec;
        }
    }
}

namespace GOSTS
{

    /// <summary>
    /// user to control textbox that input amount
    /// </summary>
    public class DecimalTextBz
    {
        private TextBox _tbNum;
        public TextBox tbNum
        {
            get { return _tbNum; }
            set { _tbNum = value; 
                //DataObject.AddPastingHandler(_tbNum, tbDecimal_Pasting);
            }
        }

        private string _proCode = "";
        public string prodCode { 
            get { return _proCode; } 
            set { _proCode = value; } 
        }

        public void SetDecText(TextBox tb)
        {
            if (tb == null) return;
            this.tbNum = tb;
            tb.PreviewKeyDown += tbDecimal_PreviewKeyDown;
          //  tb.KeyUp += tbDecimal_KeyUp;
        }

        private bool _bChkAO = false;
        public bool bChkAO
        {
            get { return _bChkAO; }
            set { _bChkAO = value; }
        }

        private int _decLen = 0;
        public int decLen
        {
            get { return _decLen; }//if prodcode 's dec length is change in real time,change to:
            set { _decLen = value; }
        }

        private void tbDecimal_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!System.Text.RegularExpressions.Regex.IsMatch(text, @"^[\-]{0,1}\d+(\.\d+)*$"))
                { e.CancelCommand(); }
            }
            else { e.CancelCommand(); }
        }

        private void tbDecimal_KeyUp(object sender, KeyEventArgs e)
        {
            string str = this.tbNum.Text.Trim();
            int i = this.tbNum.SelectionStart;
            decimal decNum = 0.0M;
            if (str == "")
            {
                return;
            }
            if (str == "-")
            {
                return;
            }
            try
            {
               decNum= Convert.ToDecimal(str);
            }
            catch
            {
                bool isNagative = false;
                if (str.StartsWith("-"))
                {
                    isNagative = true;
                    str = str.Substring(1);
                    if (i > str.Length)
                    {
                        i = str.Length;
                    }
                }

                int curPo = 0;
                if (i > 0)
                {
                    string strToCursor = str.Substring(0, i);
                    strToCursor = System.Text.RegularExpressions.Regex.Replace(strToCursor, @"[^\d\.]", "");
                    curPo = strToCursor.Length;
                }
                str = System.Text.RegularExpressions.Regex.Replace(str, @"[^\d\.]", "");
                decimal d = Utility.ConvertToDecimal(str);
                if (isNagative)
                {
                    d = -d;
                    curPo += 1;
                }
                d = GosBzTool.CheckPriceBound(d);
                str = d.ToString();

                this.tbNum.Text = str;
                if (curPo > str.Length)
                {
                    curPo = str.Length;
                }
                this.tbNum.SelectionStart = curPo;
                return;

            }
            //check if input over up bound and down bound
            if (GosBzTool.CheckPriceOverBound(decNum))
            {
                decNum = GosBzTool.CheckPriceBound(decNum);
                str = decNum.ToString();
                this.tbNum.Text = str;
                if (i < str.Length)
                {
                    i = str.Length;
                }
                this.tbNum.SelectionStart = i;
                return;
            }

            //check if dec length of input more than server specified 
            int l = GosBzTool.getDecLen(prodCode);
            
            int pos = str.IndexOf('.');
            if (pos > 0)
            {
                int decLen = str.Length - pos - 1;
                if (decLen > l)
                {
                    str = str.Substring(0, str.Length - (decLen - l));
                    if (l < 1)
                    {
                        str = str.Replace(".", "");
                    }
                    this.tbNum.Text = str;
                    if (i > str.Length)
                    {
                        i = str.Length;
                    }
                    this.tbNum.SelectionStart = i;
                }
            }
           
            //  e.Handled = true;
        }

        private void tbDecimal_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.OemMinus || e.Key == Key.Subtract || e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Enter || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                //e.Handled = true;
                //return;
            }
            else
            {
                
                string strCode = e.Key.ToString();
                if (e.Key != Key.OemPeriod && e.Key != Key.Decimal)
                if (!System.Text.RegularExpressions.Regex.IsMatch(strCode, @"(\d|\.|\-)"))//  isNumberic(e.Key.ToString()))
                {
                    e.Handled = true;
                }
            }
        }
        
        public void SetProdCode(string _prodCode)
        {
            //set prodcode , and relative decimal length, step
            if (_prodCode == null) return;
            if (_prodCode.Trim() == this.prodCode)
            {
                return;
            }
            this.prodCode = _prodCode;
            this.decLen = GosBzTool.getDecLen(prodCode);//if decimal length changes real time,put this to get property of decLen;
            if (decLen < 0)
            {
                decLen = 0;
            }
            if (decLen > 0)
            {
                decimal dec = Convert.ToDecimal(Math.Pow(10, decLen));
                dec = 1 / dec;
                this.step = dec;
            }
            else
            {
                this.step = 1;
            }
        }

        void Increase(decimal i)
        {
            decimal dec = get_txt_Price_NT();
            dec += i;
            this.tbNum.Text = dec.ToString();
            this.tbNum.SelectionStart = this.tbNum.Text.Length;
        }


        public decimal get_txt_Price_NT()
        {
            //if (ChkAO == true)
            //{
            //    return 0;
            //}
            try
            {
                return convertPrice(this.tbNum.Text.Trim()); //  Convert.ToInt32(this.txt_Price_NT.Text.Trim());
            }
            catch
            {
                return 0;
            }         
        }

        decimal convertPrice(string strPrice)
        {
            if (strPrice.EndsWith("."))
            {
                strPrice += "0";
            }
            return Convert.ToDecimal(strPrice);
        }

        public void BindWheel()
        {
            if (GOSTradeStation.customizeData.AlertData.isEnableWheel)
            {
                for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
                {
                    if (App.Current.Windows[intCounter] is GOSTradeStation)
                    {
                        App.Current.Windows[intCounter].PreviewMouseWheel += txtAmount_MouseWheel;
                        this.tbNum.PreviewMouseDown += tbNum_PreviewMouseDown;
                    }
                }
            }
        }

        void txtAmount_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!this.tbNum.IsKeyboardFocused)
            {
                return;
            }

            MdiContainer Container = App.Current.MainWindow.FindName("Container") as MdiContainer;
            //2014-03-06 CANCEL
            if (mdiChild != null && Container != null)
            {
                if (mdiChild != Container.ActiveMdiChild)
                {
                   // if (!DecimalTextBz.ExceptActiveMdiChild())
                    {
                        return;
                    }
                }
            }

            int i = e.Delta;
            int time = i / 100;
            if (bChkAO == true)
            {
                return;
            }
            decimal d = step *time  ;
            Increase(d);
            e.Handled = true;
        }

        private decimal _step = 1;
        public decimal step
        {
            get;
            set;
        }

        void tbNum_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MdiContainer Container = App.Current.MainWindow.FindName("Container") as MdiContainer;
            if (mdiChild != null && Container != null)
            {
                if (mdiChild != Container.ActiveMdiChild)
                {
                    mdiChild.Focus();
                }
            }
        }       

        public void UnBindWheel()
        {
            if (GOSTradeStation.customizeData.AlertData.isEnableWheel)
            {
                for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
                {
                    if (App.Current.Windows[intCounter] is GOSTradeStation)
                    {
                        App.Current.Windows[intCounter].PreviewMouseWheel -= txtAmount_MouseWheel;
                    }
                }
            }
        }

        /// <summary>
        /// for adjust current active is _mdichild when mouse wheel at price textbox
        /// </summary>
        private MdiChild mdiChild;
        public void SetMdiChild(MdiChild _mdiChild)
        {
            mdiChild = _mdiChild;
        }

        public static bool ExceptActiveMdiChild()
        {
            return false;
        }
    }
}
