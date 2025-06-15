# WpfSkia

This is an attempt to get SkiaSharp to work with WPF.

It works, but there is no hardware acceleration.

If you run it, you can see that it utilizes both CPU and GPU, however:

- The GPU consumption can probably be explained in full by the fact that at the end of rendering every frame, the WritableBitmap has to be copied to the video RAM.  That's probably the only hardware-accelerated operation that takes place, and it is WPF that does it, not Skia.

- All the drawing appears to be done in software, because if you enlarge the window, the CPU consumption increases. The only thing I do in software which depends on the window size is picking larger coordinates; therefore, the increased CPU consumption can only mean that software is involved in drawing the lines, so the longer the lines are, the more the CPU has to work.

I tried to use the OpenGL control, but it fails with some ridiculous error like "this driver does not appear to support OpenGL".

So, the final verdict is that SkiaSharp is an immature technology (not a single class, property, method, or parameter to method has a single line of documentation,) and hardware acceleration in Skia is held together with shoestrings.
