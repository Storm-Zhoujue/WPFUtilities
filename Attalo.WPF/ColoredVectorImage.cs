using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Attalo.WPF
{

    [TemplatePart(Name = ImagePart, Type = typeof(Image))]
    public class ColoredVectorImage : Control, ICommandSource
    {

        private const string ImagePart = "PART_Image";

        static ColoredVectorImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColoredVectorImage), 
                new FrameworkPropertyMetadata(typeof(ColoredVectorImage)));

            FrameworkPropertyMetadata meta = new FrameworkPropertyMetadata();
            meta.DefaultValue = new SolidColorBrush(Colors.Black);
            meta.AffectsMeasure = false;
            meta.AffectsRender = false;
            ColorProperty = DependencyProperty.Register("Color", typeof(SolidColorBrush), typeof(ColoredVectorImage), meta);


            CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ColoredVectorImage),
                new PropertyMetadata((ICommand)null, new PropertyChangedCallback(CommandChanged)));


        }

        private Image _image;
        protected Image Image
        {
            get { return _image; }
            set
            {
                _image = value;
                if (_image != null)
                {
                    _image.Source = this.ImageSource;
                }
            }
        }



        public SolidColorBrush Color
        {
            get { return (SolidColorBrush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty;

        public DrawingImage ImageSource
        {
            get { return (DrawingImage)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        #region ICommandSource


        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty;

        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(ColoredVectorImage), new PropertyMetadata(null));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(ColoredVectorImage), new PropertyMetadata(default(object)));

        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColoredVectorImage cvi = (ColoredVectorImage)d;
            cvi.HookUpCommand((ICommand)e.OldValue, (ICommand)e.NewValue);
        }

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public IInputElement CommandTarget
        {
            get { return (IInputElement)GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }

        // Add a new command to the Command Property.
        private void HookUpCommand(ICommand oldCommand, ICommand newCommand)
        {
            // If oldCommand is not null, then we need to remove the handlers.
            if (oldCommand != null)
            {
                RemoveCommand(oldCommand, newCommand);
            }
            AddCommand(oldCommand, newCommand);
        }

        // Remove an old command from the Command Property.
        private void RemoveCommand(ICommand oldCommand, ICommand newCommand)
        {
            EventHandler handler = CanExecuteChanged;
            oldCommand.CanExecuteChanged -= handler;
        }

        // Add the command.
        private void AddCommand(ICommand oldCommand, ICommand newCommand)
        {
            EventHandler handler = new EventHandler(CanExecuteChanged);
            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += handler;
            }
        }

        private void CanExecuteChanged(object sender, EventArgs e)
        {

            if (this.Command != null)
            {
                RoutedCommand command = this.Command as RoutedCommand;

                // If a RoutedCommand.
                if (command != null)
                {
                    if (command.CanExecute(CommandParameter, CommandTarget))
                    {
                        this.IsEnabled = true;
                    }
                    else
                    {
                        this.IsEnabled = false;
                    }
                }
                // If a not RoutedCommand.
                else
                {
                    if (Command.CanExecute(CommandParameter))
                    {
                        this.IsEnabled = true;
                    }
                    else
                    {
                        this.IsEnabled = false;
                    }
                }
            }
        }

        //invoke the command
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (this.Command != null && this.IsEnabled)
            {
                if (Command is RoutedCommand command)
                {
                    command.Execute(CommandParameter, CommandTarget);
                }
                else
                {
                    ((ICommand)Command).Execute(CommandParameter);
                }
            }
        }

        #endregion

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(DrawingImage), typeof(ColoredVectorImage), new PropertyMetadata(default(DrawingImage)));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (ImageSource.Drawing is DrawingGroup group)
            {
                foreach (var item in group.Children)
                {
                    if (item is GeometryDrawing geo)
                    {
                        geo.Brush = this.Color;
                    }
                }
            }
            else
            {
                if (ImageSource.Drawing is GeometryDrawing geo)
                {
                    geo.Brush = this.Color;
                }
            }

            if (this.Template != null)
            {
                Image = GetTemplateChild(ImagePart) as Image;
            }
        }



    }
}
