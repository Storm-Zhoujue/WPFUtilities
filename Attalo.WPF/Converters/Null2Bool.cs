﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Attalo.WPF
{
    public class Null2Bool : MarkupExtension, IValueConverter
    {

        public Null2Bool()
        {
  
        }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }



}