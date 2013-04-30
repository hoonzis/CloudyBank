using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using CloudyBank.Web.Ria.ViewModels;
using System.Collections.ObjectModel;
using System.Collections;

namespace CloudyBank.Web.Ria.Views
{
    public partial class Account : UserControl
    {
        public Account()
        {
            InitializeComponent();
        }
        
        private void TextBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textblock = (TextBlock)sender;
            OperationViewModel operation = textblock.DataContext as OperationViewModel;
            ContextMenu contextMenu = new ContextMenu();
            CustomerViewModel vm = Customer.Data as CustomerViewModel;

            if (vm.Tags != null)
            {
                foreach (TagViewModel tag in vm.Tags)
                {
                    contextMenu.Items.Add(new MenuItem { Header = tag.Title, Command = operation.TagCommand, CommandParameter = tag });
                }
            }

            contextMenu.Items.Add(new Separator());
            
            var referentialVM = App.Current.Resources["Referential"] as ReferentialDataViewModel;

            foreach (TagViewModel tag in referentialVM.StandardTags)
            {
                contextMenu.Items.Add(new MenuItem { Header = tag.Title, Command = operation.TagCommand, CommandParameter = tag });
            }

            ContextMenuService.SetContextMenu(textblock, contextMenu);

            contextMenu.HorizontalOffset = e.GetPosition(null).X;
            contextMenu.VerticalOffset = e.GetPosition(null).Y;
            contextMenu.IsOpen = true;

            e.Handled = true;
        }
    }
}
