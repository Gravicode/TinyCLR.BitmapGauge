// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
var gauge = new BitmapGauge.BitmapGauge(500,500);
gauge.Value = 70;
gauge.ThresholdPercent = 50;
var bmp = gauge.GetGauge();
bmp.Save(Directory.GetCurrentDirectory() + "\\gauge.jpg");