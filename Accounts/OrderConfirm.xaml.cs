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
using WPF.MDI;
using GOSTS.Common;

namespace GOSTS
{
    /// <summary>
    /// OrderConfirm.xaml 的交互逻辑
    /// </summary>
    public partial class OrderConfirm : UserControl
    {
        MdiChild mdi_Close;

        public OrderConfirm(MdiChild windows)
        {
            InitializeComponent();
            mdi_Close = windows;
            mdi_Close.Closing += new RoutedEventHandler(mdi_Close_Closing);
        }

        public void mdi_Close_Closing(object sender, RoutedEventArgs e)
        {
            if (!GOSTradeStation.notifications.IsEmpty())
            {
                ClosingEventArgs eventArgs = e as ClosingEventArgs;
                eventArgs.Cancel = true;

                this.Notifications_Opening(sender, e);
            }
            else
            {
                this.mdi_Close.Close();
            }
        }

        public void Notifications_Opening(object sender, RoutedEventArgs e)
        {
            //TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + " OrderConfirm Count:" + GOSTradeStation.notifications.Count);

            if (GOSTradeStation.notifications.IsEmpty())
            {
                this.mdi_Close.Close();
            }
            else
            {
                Object obj = GOSTradeStation.notifications.Dequeue();
                Type type = obj.GetType();
                var @switch = new Dictionary<Type, Action> 
            { 
              {typeof(OrderResponse ), () =>
                  { 
                    OrderResponse  orderResponse =obj  as OrderResponse;
                    if(orderResponse.Type == "Frist")
                    {
                        if (GOSTradeStation.notifications.IsEmpty())
                        {
                            this.mdi_Close.Close();
                        }
                        else
                        {
                            this.Notifications_Opening(sender, e);
                        }
                    }
                    else
                    {
                        OrderConfirm ocf = this.mdi_Close.Content as OrderConfirm;
                        ocf.lbl_Message.Text = orderResponse.Message;
                        if (orderResponse.Result < 0)
                        {
                            ocf.lbl_Message.Foreground = Brushes.Red;
                        }
                        else
                        {
                            ocf.lbl_Message.Foreground = Brushes.Green;
                        }
                        this.mdi_Close.Title = GOSTradeStation.UserID + " - Order Confirm";
                    }
                  }
                }, 
            {typeof(Notification), () => 
                {
                    Notification notification = obj as Notification;
                    if (notification.Type == "Frist")
                    {
                        if (GOSTradeStation.notifications.IsEmpty())
                        {
                            this.mdi_Close.Close();
                        }
                        else
                        {
                            this.Notifications_Opening(sender, e);
                        }
                    }
                    else
                    {
                        OrderConfirm ocf = this.mdi_Close.Content as OrderConfirm;
                        ocf.lbl_Message.Text = notification.AllMessage;
                        if (notification.Notify_Code == (int)NotifyTypeCode.OrderActivateFailure ||
                           notification.Notify_Code == (int)NotifyTypeCode.OrderAddFailure ||
                           notification.Notify_Code == (int)NotifyTypeCode.OrderChangeFailure ||
                           notification.Notify_Code == (int)NotifyTypeCode.OrderDeleteFailure)
                        {
                            ocf.lbl_Message.Foreground = Brushes.Red;
                        }
                        else
                        {
                            ocf.lbl_Message.Foreground = Brushes.Green;
                        }
                        this.mdi_Close.Title = notification.UserID + " - Notification Order Confirm";
                    }
                }
            },
           {typeof(TableNotification ), () => 
             {
                 TableNotification tableNotification = obj as TableNotification;
                 if (tableNotification.Type == "Frist")
                    {
                        if (GOSTradeStation.notifications.IsEmpty())
                        {
                            this.mdi_Close.Close();
                        }
                        else
                        {
                            this.Notifications_Opening(sender, e);
                        }
                    }
                    else
                    {
                        OrderConfirm ocf = this.mdi_Close.Content as OrderConfirm;
                        ocf.lbl_Message.Text = tableNotification.AllMessage;
                        ocf.lbl_Message.Foreground = Brushes.Green;
                        this.mdi_Close.Title = tableNotification.UserID + " - TableNotification Order Confirm";
                    }
             }
             },
            };
                @switch[type]();
            }
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            if (mdi_Close != null)
            {
                this.Notifications_Opening(sender, e);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.mdi_Close.Width = this.mdi_Close.ActualWidth;
            this.mdi_Close.Height = this.mdi_Close.ActualHeight;
           // this.mdi_Close.Resizable = false;
        }

    }
}

 
