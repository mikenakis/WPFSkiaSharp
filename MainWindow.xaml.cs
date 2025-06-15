namespace WpfSkiaSharp;

using System;
using SkiaSharp.Views.Desktop;
using Skia = SkiaSharp;
using Sys = System;
using Wpf = System.Windows;

public partial class MainWindow : Wpf.Window
{
	int i;
	readonly Skia.SKPaint[] paints = new Skia.SKPaint[1000];
	readonly Sys.Random rand = new();
	bool antialias = true;
	int currentMaxAlpha = 100;
	int alpha = 100;

	public MainWindow()
	{
		InitializeComponent();
		skElement.Paint += onSkElementPaintSurface;
		Wpf.Media.CompositionTarget.Rendering += onVSync;

		for( int i = 0; i < paints.Length; i++ )
		{
			paints[i] = new Skia.SKPaint
			{
				Color = new Skia.SKColor(
					red: (byte)rand.Next( 255 ),
					green: (byte)rand.Next( 255 ),
					blue: (byte)rand.Next( 255 ),
					alpha: (byte)rand.Next( currentMaxAlpha ) ),
				StrokeWidth = rand.Next( 1, 10 ),
				IsAntialias = antialias,
				Style = Skia.SKPaintStyle.Stroke
			};
		}
	}

	private void onPaintSurface( object? sender, SKPaintGLSurfaceEventArgs e )
	{
		draw( e.Surface.Canvas, e.Info.Size.Width, e.Info.Size.Height );
	}

	void onVSync( object? sender, Sys.EventArgs e )
	{
		if( i == 0 /*|| i % 1000 == 0*/ )
			skElement.InvalidateVisual();
		else
			skElement.Redraw();
		i++;
	}

	void onSkElementPaintSurface( Skia.SKSurface surface, int width, int height )
	{
		Skia.SKCanvas canvas = surface.Canvas;
		draw( canvas, width, height );
	}

	void draw( Skia.SKCanvas canvas, int width, int height )
	{
		canvas.Clear( new Skia.SKColor( 60, 30, 30 ) );

		if( antialias != paints[0].IsAntialias )
			for( int i = 0; i < paints.Length; i++ )
				paints[i].IsAntialias = antialias;
		if( alpha != currentMaxAlpha )
		{
			currentMaxAlpha = alpha;
			for( int i = 0; i < paints.Length; i++ )
				paints[i].Color = paints[i].Color.WithAlpha( (byte)rand.Next( currentMaxAlpha ) );
		}

		int lineCount = 1000;
		for( int i = 0; i < lineCount; i++ )
		{
			Skia.SKPaint paint = paints[rand.Next( paints.Length )];
			int x1 = rand.Next( width );
			int y1 = rand.Next( height );
			int x2 = rand.Next( width );
			int y2 = rand.Next( height );
			canvas.DrawLine( x1, y1, x2, y2, paint );
		}

		var bigFont = new Skia.SKFont( Skia.SKTypeface.FromFamilyName( "Microsoft YaHei UI" ), 80 );
		var smallFont = new Skia.SKFont( Skia.SKTypeface.FromFamilyName( "Microsoft YaHei UI" ), 40 );
		var verySmallFont = new Skia.SKFont( Skia.SKTypeface.FromFamilyName( "Microsoft YaHei UI" ), 16 );
		var whitePaint = new Skia.SKPaint() { Color = new Skia.SKColor( 250, 250, 250 ) };
		canvas.DrawText( $"SkiaSharp in Wpf! {i}", 50.0f, 200.0f, Skia.SKTextAlign.Left, bigFont, whitePaint );
		canvas.DrawText( "Using SkiaSharp in WPF", new Skia.SKPoint( 50, 360 ), Skia.SKTextAlign.Left, smallFont, whitePaint );
		canvas.DrawText( "For some reason SysInternals Process Explorer registers no GPU usage when this window is on my", new Skia.SKPoint( 50, 440 ), Skia.SKTextAlign.Left, verySmallFont, whitePaint );
		canvas.DrawText( "small (built-in) monitor.  Windows Task Manager registers GPU usage on either monitor.", new Skia.SKPoint( 50, 460 ), Skia.SKTextAlign.Left, verySmallFont, whitePaint );
	}
}
