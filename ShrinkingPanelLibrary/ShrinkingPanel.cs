using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Gregstoll
{
    /// <summary>
    /// A <see cref="ShrinkingPanel"/> is like a <see cref="StackPanel"/> that can optionally shrink a child element.
    /// Here's how it works:
    /// <list type="bullet">
    /// <item>
    /// <description>If all children fit in the available direction (specified by <see cref="Orientation"/>),
    /// then they all get that space.</description>
    /// </item>
    /// <item>
    /// <description>If not, the child at index <see cref="ShrinkingElementIndex"/> will be shrunk so everything
    /// fits.</description>
    /// </item>
    /// </list>
    /// The <see cref="ShrinkingPanel"/> is designed to be used if you have a child element that can do its own scrolling,
    /// like a <see cref="ListBox"/> or a <see cref="ScrollViewer"/>.  If you put that element at the
    /// <see cref="ShrinkingElementIndex"/>, then you can put controls after it that will show up, but won't get pushed to the
    /// bottom/right unless the ShrinkingElement needs all of the available space.
    /// </summary>
    public class ShrinkingPanel : Panel
    {
        /// <summary>
        /// Which orientation to lay children out in
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
          DependencyProperty.Register("Orientation",
          typeof(Orientation), typeof(ShrinkingPanel), new PropertyMetadata(Orientation.Vertical));

        /// <summary>
        /// The index of the child element to shrink if there is not enough room for all of the children.
        /// </summary>
        public int ShrinkingElementIndex
        {
            get { return (int)GetValue(ShrinkingElementIndexProperty); }
            set { SetValue(ShrinkingElementIndexProperty, value); }
        }

        public static readonly DependencyProperty ShrinkingElementIndexProperty =
          DependencyProperty.Register("ShrinkingElementIndex",
          typeof(int), typeof(ShrinkingPanel), new PropertyMetadata(0));

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            Point point = new Point(0, 0);
            for (int i = 0; i < Children.Count; ++i)
            {
                var child = Children[i];
                child.Measure(availableSize);
                if (Orientation == Orientation.Vertical)
                {
                    point.Y += child.DesiredSize.Height;
                    point.X = Math.Max(point.X, child.DesiredSize.Width);
                }
                else
                {
                    point.X += child.DesiredSize.Width;
                    point.Y = Math.Max(point.Y, child.DesiredSize.Height);
                }
            }
            if (Orientation == Orientation.Vertical)
            {
                if (point.Y > availableSize.Height && ShrinkingElementIndex < Children.Count)
                {
                    Children[ShrinkingElementIndex].Measure(new Size(availableSize.Width, Math.Max(0, Children[ShrinkingElementIndex].DesiredSize.Height - (point.Y - availableSize.Height))));
                }
                return new Size(Math.Min(point.X, availableSize.Width), Math.Min(point.Y, availableSize.Height));
            }
            else
            {
                if (point.X > availableSize.Width && ShrinkingElementIndex < Children.Count)
                {
                    Children[ShrinkingElementIndex].Measure(new Size(Math.Max(0, Children[ShrinkingElementIndex].DesiredSize.Width - (point.X - availableSize.Width)), availableSize.Height));
                }
                return new Size(Math.Min(point.X, availableSize.Width), Math.Min(point.Y, availableSize.Height));
            }
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double amountToShrink = 0;
            {
                Point point = new Point(0, 0);
                // See if we're not going to fit on our dimension
                foreach (UIElement child in Children)
                {
                    if (Orientation == Orientation.Vertical)
                    {
                        point.Y += child.DesiredSize.Height;
                        point.X = Math.Max(point.X, child.DesiredSize.Width);
                    }
                    else
                    {
                        point.X += child.DesiredSize.Width;
                        point.Y = Math.Max(point.Y, child.DesiredSize.Height);
                    }
                }
                if (Orientation == Orientation.Vertical)
                {
                    if (point.Y > finalSize.Height)
                    {
                        amountToShrink = point.Y - finalSize.Height;
                    }
                }
                else
                {
                    if (point.X > finalSize.Width)
                    {
                        amountToShrink = point.X - finalSize.Width;
                    }
                }
            }

            {
                Point point = new Point(0, 0);
                for (int i = 0; i < Children.Count; ++i)
                {
                    UIElement child = Children[i];
                    Size childSize;
                    if (i == ShrinkingElementIndex && amountToShrink > 0)
                    {
                        if (Orientation == Orientation.Vertical)
                        {
                            childSize = new Size(child.DesiredSize.Width, Math.Max(0, child.DesiredSize.Height - amountToShrink));
                        }
                        else
                        {
                            childSize = new Size(Math.Max(0, child.DesiredSize.Width - amountToShrink), child.DesiredSize.Height);
                        }
                    }
                    else
                    {
                        childSize = child.DesiredSize;
                    }
                    child.Arrange(new Rect(point, childSize));
                    if (Orientation == Orientation.Vertical)
                    {
                        point.Y += childSize.Height;
                    }
                    else
                    {
                        point.X += childSize.Width;
                    }
                }
            }
            return finalSize;
        }
    }
}
