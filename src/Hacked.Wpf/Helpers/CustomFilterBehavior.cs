using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Telerik.Windows.Controls;

namespace Hacked.Wpf.Helpers
{
    public class CustomFilterBehavior
    {
        private static DispatcherTimer _timer;
        private readonly RadGridView _gridView;
        private readonly RadWatermarkTextBox _textBlock;
        private readonly RadBusyIndicator _busyIndicator;
        private CustomFilterDescriptor _filterDescriptor;

        static CustomFilterBehavior()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1),
            };
        }

        public static readonly DependencyProperty TextBoxProperty =
            DependencyProperty.RegisterAttached("TextBox", typeof(TextBox), typeof(CustomFilterBehavior),
            new PropertyMetadata(new PropertyChangedCallback(OnTextBoxPropertyChanged)));

        public CustomFilterDescriptor FilterDescriptor
        {
            get
            {
                if (_filterDescriptor == null)
                {
                    _filterDescriptor = new CustomFilterDescriptor(_gridView.Columns.OfType<Telerik.Windows.Controls.GridViewColumn>());
                    _gridView.FilterDescriptors.Add(_filterDescriptor);
                }
                return _filterDescriptor;
            }
        }

        public static void SetTextBox(DependencyObject dependencyObject, TextBox tb)
        {
            dependencyObject.SetValue(TextBoxProperty, tb);
        }

        public static TextBox GetTextBox(DependencyObject dependencyObject)
        {
            return (TextBox)dependencyObject.GetValue(TextBoxProperty);
        }

        public static void OnTextBoxPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var gridView = dependencyObject as RadGridView;
            var textBlock = e.NewValue as RadWatermarkTextBox;
            var busyIndicator = gridView.ParentOfType<RadBusyIndicator>();

            if (gridView != null && textBlock != null)
            {
                var behavior = new CustomFilterBehavior(gridView, textBlock, busyIndicator);
            }
        }

        public CustomFilterBehavior(RadGridView gridView, RadWatermarkTextBox textBlock, RadBusyIndicator busyIndicator)
        {
            _gridView = gridView;
            _textBlock = textBlock;
            _busyIndicator = busyIndicator;

            _textBlock.TextChanged -= OnTextBlockTextChanged;
            _textBlock.TextChanged += OnTextBlockTextChanged;
        }

        private void SetStatusBusyIndicator(bool isBusy)
        {
            if (_busyIndicator != null)
            {
                _busyIndicator.IsBusy = isBusy;
            }
        }

        private void OnTextBlockTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_timer != null && _timer.IsEnabled)
            {
                _timer.Stop();
                _timer.Start();
            }
            else
            {
                if (_timer != null)
                {
                    _timer.Start();
                    _timer.Tick += OnTimerTick;
                }
            }

            SetStatusBusyIndicator(true);
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            SetStatusBusyIndicator(false);
            FilterDescriptor.FilterValue = _textBlock.Text;
            _textBlock.Focus();
        }
    }
}