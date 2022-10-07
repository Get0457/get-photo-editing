﻿#nullable enable
using System;
using System.Text;
using Windows.UI.Xaml;
using Windows.Foundation;
using Newtonsoft.Json.Linq;
using Windows.ApplicationModel.DataTransfer;
using System.Diagnostics;

namespace PhotoFlow;

partial class MainPage
{
    async void OpenFile(object _, RoutedEventArgs _1)
    {
        var (fileType, bytes) = await FileManagement.OpenFile();
        switch (fileType)
        {
            case "<Canceled>":
                break;
            case Constants.GetPhotoEditingFileFormat:
                Debug.Assert(bytes != null);
                await LayerContainer.LoadAndReplace(JObject.Parse(Encoding.UTF8.GetString(bytes)));
                break;
            default:
                SetNewImage(bytes!.ToMat());
                break;
        }
    }
    void SaveFile(object _, RoutedEventArgs _1)
    {
        _ = FileManagement.SaveFile(LayerContainer, LayerContainerMasker, LayerContainerBackground);
    }
    private async void InsertFile(object _, RoutedEventArgs _1)
    {
        var mat = await FileManagement.OpenNewImageFile();
        var layer = new Layers.MatLayer(mat);
        layer.LayerName.Value = "Inserted Image";
        LayerContainer.AddNewLayer(layer);
    }

    async void NewFromClipboard(object sender, RoutedEventArgs e)
    {
        var layer = await Clipboard.GetContent().ReadAsLayerAsync();
        if (layer != null) NewImage(layer);
        else
        {
            await new ThemeContentDialog
            {
                Title = "Error",
                Content = "There is no compatable Clipboard Item!",
                PrimaryButtonText = "Okay"
            }.ShowAsync();
        }
    }
    async void New(object sender, RoutedEventArgs e)
    {
        var dialog = new NewDialog();
        await dialog.ShowAsync();
        if (!dialog.Success) return;
        if (dialog.FromClipboard)
        {
            NewFromClipboard(sender, e);
        }
        else
        {
            var width = dialog.ImageWidth;
            var height = dialog.ImageHeight;
            var CanvasDimension = new OpenCvSharp.Size(width, height);
            var Color = dialog.InitBackground switch
            {
                "Transparent" => Windows.UI.Color.FromArgb(0, 0, 0, 0),
                "White" => Windows.UI.Color.FromArgb(255, 255, 255, 255),
                "Black" => Windows.UI.Color.FromArgb(255, 0, 0, 0),
                _ => throw new Exception("impossible!"),
            };
            NewImage(Color, CanvasDimension);
        }
    }
    void NewImage(Windows.UI.Color Color, OpenCvSharp.Size CanvasDimension)
    {
        Size Size = new(CanvasDimension.Width, CanvasDimension.Height);
        var layer = new Layers.RectangleLayer()
        {
            Color = Color,
            Width = Size.Width,
            Height = Size.Height
        };
        layer.LayerName.Value = "Background";
        NewImage(layer);
    }
    void NewImage(Layers.Layer Layer)
    {
        CanvasDimension = new OpenCvSharp.Size(Layer.Width, Layer.Height);

        Size Size = new(CanvasDimension.Width, CanvasDimension.Height);
        LayerContainer.ImageSize = Size;

        LayerContainer.Clear();
        Layer.LayerName.Value = "Background";
        LayerContainer.AddNewLayer(Layer);
    }
    void SetNewImage(OpenCvSharp.Mat Image)
    {
        if (Image.Channels() == 3) Image = Image.CvtColor(OpenCvSharp.ColorConversionCodes.BGR2BGRA);

        var CvSize = new OpenCvSharp.Size(Image.Width, Image.Height);
        CanvasDimension = CvSize;

        Size Size = new(CanvasDimension.Width, CanvasDimension.Height);
        LayerContainer.ImageSize = Size;

        LayerContainer.Clear();
        var layer = new Layers.MatLayer(Image);
        layer.LayerName.Value = "Background";
        LayerContainer.AddNewLayer(layer);
    }
}
