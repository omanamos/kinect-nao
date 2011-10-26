using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;

namespace MotionRecorder
{
    public delegate void SliderValueChangedEventHandler(object sender, RoutedPropertyChangedEventArgs<double> e);
    class SliderManager
    {
        public event SliderValueChangedEventHandler ValueChanged;

        public SliderManager(Slider s)
        {
            s.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(Slider_DragCompleted));
            s.AddHandler(Thumb.DragStartedEvent, new DragStartedEventHandler(Slider_DragStarted));
        }

        private bool dragStarted = false;
        private void Slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            ValueChanged(sender, new RoutedPropertyChangedEventArgs<double>(0, ((Slider)sender).Value));
            this.dragStarted = false;
        }
        private void Slider_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.dragStarted = true;
        }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStarted)
                ValueChanged(sender, e);
        }
    }
}
