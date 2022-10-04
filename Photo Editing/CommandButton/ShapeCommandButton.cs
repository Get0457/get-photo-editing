﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.Devices.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI;
using PhotoFlow.CommandButton.Controls;

namespace PhotoFlow
{
    public class ShapeCommandButton : CommandButtonBase
    {
        private readonly Shape ShapeCommandBar = new ();
        protected override CommandButtonCommandBar CommandBar => ShapeCommandBar;

        public ShapeCommandButton(Border CommandBarPlace, LayerContainer LayerContainer, ScrollViewer MainScrollViewer) : base(Symbol.Stop, CommandBarPlace, LayerContainer, MainScrollViewer)
        {
            ShapeCommandBar.CreateRectangle.Click += delegate
            {
                AddNewLayer(new Layer.RectangleLayer
                {
                    Width = 100,
                    Height = 100,
                    Color = Colors.Black
                }.SetName("Rectangle"));
            };
            ShapeCommandBar.CreateEllipse.Click += delegate
            {
                AddNewLayer(new Layer.EllipseLayer
                {
                    Width = 100,
                    Height = 100,
                    Color = Colors.Black
                }.SetName("Ellipse"));
            };
            ShapeCommandBar.Acrylic.Checked += delegate
            {
                if (CurrentLayer is Layer.ShapeLayer ShapeLayer)
                {
                    ShapeLayer.Acrylic = true;
                    ShapeCommandBar.TintOpacityField.Value = ShapeLayer.TintOpacity * 100;
                }
            };
            ShapeCommandBar.Acrylic.Unchecked += delegate
            {
                if (CurrentLayer is Layer.ShapeLayer ShapeLayer) ShapeLayer.Acrylic = false;
            };
            ShapeCommandBar.ColorPicker.ColorChanged += delegate
            {
                if (CurrentLayer is Layer.ShapeLayer ShapeLayer) ShapeLayer.Color = ShapeCommandBar.ColorPicker.Color;
            };
            
            ShapeCommandBar.OpacityField.ValueChanged += delegate
            {
                if (CurrentLayer is Layer.ShapeLayer ShapeLayer) ShapeLayer.Opacity = ShapeCommandBar.OpacityField.Value / 100;
            };
            ShapeCommandBar.TintOpacityField.ValueChanged += delegate
            {
                if (CurrentLayer is Layer.ShapeLayer ShapeLayer) ShapeLayer.TintOpacity = ShapeCommandBar.TintOpacityField.Value / 100;
            };
        }
        protected override void LayerChanged(Layer.Layer Layer)
        {
            base.LayerChanged(Layer);
            if (Layer == null) return;
            ShapeCommandBar.LayerEditorControls.Visibility =
                Layer is Layer.ShapeLayer ? Visibility.Visible : Visibility.Collapsed;
            if (Layer is Layer.ShapeLayer ShapeLayer)
            {
                ShapeCommandBar.Acrylic.IsChecked = ShapeLayer.Acrylic;
                ShapeCommandBar.ColorPicker.Color = ShapeLayer.Color;
                ShapeCommandBar.OpacityField.Value = ShapeLayer.Opacity * 100;
                ShapeCommandBar.TintOpacityField.Value = ShapeLayer.TintOpacity * 100;
                ShapeCommandBar.PropertiesButton.Layer = ShapeLayer;
            }
        }
    }
}
