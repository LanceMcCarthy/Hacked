using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hacked.Controls;

public class AdaptiveGridView : GridView
{
    #region DependencyProperties

    /// <summary>
    /// Minimum height for item (must be greater than zero)
    /// </summary>
    public double MinItemHeight
    {
        get => (double)GetValue(MinItemHeightProperty);
        set => SetValue(MinItemHeightProperty, value);
    }

    public static readonly DependencyProperty MinItemHeightProperty =
        DependencyProperty.Register(
            nameof(MinItemHeight),
            typeof(double),
            typeof(AdaptiveGridView),
            new PropertyMetadata(1.0, (s, a) =>
            {
                if (!double.IsNaN((double)a.NewValue))
                {
                    ((AdaptiveGridView)s).InvalidateMeasure();
                }
            }));

    /// <summary>
    /// Minimum width for item (must be greater than zero)
    /// </summary>
    public double MinItemWidth
    {
        get => (double)GetValue(MinimumItemWidthProperty);
        set => SetValue(MinimumItemWidthProperty, value);
    }

    public static readonly DependencyProperty MinimumItemWidthProperty =
        DependencyProperty.Register(
            nameof(MinItemWidth),
            typeof(double),
            typeof(AdaptiveGridView),
            new PropertyMetadata(1.0, (s, a) =>
            {
                if (!double.IsNaN((double)a.NewValue))
                {
                    ((AdaptiveGridView)s).InvalidateMeasure();
                }
            }));

    /// <summary>
    /// Reports if there are currently any items in the AdaptiveGridView, is updated as items change.
    /// This property can be used to bind to the Visibility of an "Empty Content" overlay.
    /// </summary>
    public bool HasItems
    {
        get => (bool)GetValue(HasItemsProperty);
        set => SetValue(HasItemsProperty, value);
    }

    public static readonly DependencyProperty HasItemsProperty = DependencyProperty.Register(
        nameof(HasItems), typeof(bool), typeof(AdaptiveGridView), new PropertyMetadata(default(bool)));

    #endregion

    public AdaptiveGridView()
    {
        ItemContainerStyle ??= new Style(typeof(GridViewItem));

        ItemContainerStyle.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));

        Loaded += AdaptiveGridView_Loaded;
    }

    private void AdaptiveGridView_Loaded(object sender, RoutedEventArgs e)
    {
        if (ItemsPanelRoot != null)
        {
            InvalidateMeasure();
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (ItemsPanelRoot is ItemsWrapGrid panel)
        {
            if (Math.Abs(MinItemWidth) < 0.01 || Math.Abs(MinItemHeight) < 0.01)
            {
                throw new ArgumentException("You need to set MinItemHeight and MinItemWidth to a value greater than 0");
            }

            var availableWidth = finalSize.Width - (Padding.Right + Padding.Left);

            var numColumns = Math.Floor(availableWidth / MinItemWidth);
            numColumns = Math.Abs(numColumns) < 0.01 ? 1 : numColumns;

            //Not used yet (for horizontal scrolling scenarios)
            //var numRows = Math.Ceiling(this.Items.Count / numColumns);

            var itemWidth = availableWidth / numColumns;
            var aspectRatio = MinItemHeight / MinItemWidth;
            var itemHeight = itemWidth * aspectRatio;

            panel.ItemWidth = itemWidth;
            panel.ItemHeight = itemHeight;
        }

        return base.ArrangeOverride(finalSize);
    }

    protected override void OnItemsChanged(object e)
    {
        base.OnItemsChanged(e);

        HasItems = Items?.Count > 0;
    }
}
