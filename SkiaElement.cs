namespace WpfSkiaSharp;

using Skia = SkiaSharp;
using Sys = System;
using SysCompModel = System.ComponentModel;
using Wpf = System.Windows;
using WpfImaging = System.Windows.Media.Imaging;
using WpfMedia = System.Windows.Media;

[SysCompModel.DefaultEvent( "PaintSurface" )]
[SysCompModel.DefaultProperty( "Name" )]
public class SkiaElement : Wpf.FrameworkElement
{
	public delegate void PaintEventHandler( Skia.SKSurface surface, int width, int height );

	private const double bitmapDpi = 96.0;

	private readonly bool designMode;
	private double scaleX = 1.0;
	private double scaleY = 1.0;
	private WpfImaging.WriteableBitmap? bitmap;

	[SysCompModel.Category( "Appearance" )]
	public event PaintEventHandler? Paint;

	public SkiaElement()
	{
		designMode = SysCompModel.DesignerProperties.GetIsInDesignMode( this );
	}

	public int CanvasWidth { get; private set; }
	public int CanvasHeight { get; private set; }

	protected override void OnRender( WpfMedia.DrawingContext drawingContext )
	{
		base.OnRender( drawingContext );

		if( designMode || !isPositive( ActualWidth ) || !isPositive( ActualHeight ) || Visibility != Wpf.Visibility.Visible || Wpf.PresentationSource.FromVisual( this ) == null )
			return;

		WpfMedia.Matrix deviceTransformMatrix = Wpf.PresentationSource.FromVisual( this ).CompositionTarget.TransformToDevice;
		scaleX = deviceTransformMatrix.M11;
		scaleY = deviceTransformMatrix.M22;
		CanvasWidth = (int)(ActualWidth * scaleX);
		CanvasHeight = (int)(ActualHeight * scaleY);
		Redraw();
		drawingContext.DrawImage( bitmap, new Wpf.Rect( 0, 0, ActualWidth, ActualHeight ) );
	}

	public void Redraw()
	{
		if( bitmap == null || CanvasWidth != bitmap.PixelWidth || CanvasHeight != bitmap.PixelHeight )
			bitmap = new WpfImaging.WriteableBitmap( CanvasWidth, CanvasHeight, bitmapDpi * scaleX, bitmapDpi * scaleY, WpfMedia.PixelFormats.Pbgra32, null );
		bitmap.Lock();
		using( var surface = Skia.SKSurface.Create( new Skia.SKImageInfo( CanvasWidth, CanvasHeight ), bitmap.BackBuffer, bitmap.BackBufferStride ) )
			Paint?.Invoke( surface, CanvasWidth, CanvasHeight );
		bitmap.AddDirtyRect( new Wpf.Int32Rect( 0, 0, CanvasWidth, CanvasHeight ) );
		bitmap.Unlock();
	}

	protected override void OnRenderSizeChanged( Wpf.SizeChangedInfo sizeInfo )
	{
		base.OnRenderSizeChanged( sizeInfo );
		InvalidateVisual();
	}

	static bool isPositive( double value ) => !double.IsNaN( value ) && !double.IsInfinity( value ) && value > 0;

	public static void Assert( bool condition )
	{
		if( condition )
			return;
		throw new Sys.Exception();
	}
}
