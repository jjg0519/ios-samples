// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using UIKit;
using CoreAnimation;

namespace MetalTexturedQuad
{
	public partial class ImageViewController : UIViewController
	{
		CADisplayLink timer;
		double timeSinceLastDrawPreviousTime;
		bool gameLoopPaused;
		bool firstDrawOccurred;
		double timeSinceLastDraw;

		Renderer renderer;

		NSObject didEnterBackgroundObserver;
		NSObject willEnterForegroundObserver;

		bool Paused { 
			get {
				return gameLoopPaused;
			} 

			set {
				if (gameLoopPaused == value)
					return;

				if (timer == null)
					return;

				gameLoopPaused = timer.Paused = value;
			}
		}

		Renderer Renderer { get; set; }

		public ImageViewController (IntPtr handle) : base (handle)
		{
		}

		public void DispatchGameLoop ()
		{
			timer = CADisplayLink.Create (Gameloop);
			timer.FrameInterval = 1;
			timer.AddToRunLoop (NSRunLoop.Main, NSRunLoop.NSDefaultRunLoopMode);
		}

		public void Gameloop ()
		{
			if (!firstDrawOccurred) {
				timeSinceLastDraw = 0.0;
				timeSinceLastDrawPreviousTime = CAAnimation.CurrentMediaTime ();
				firstDrawOccurred = true;
			} else {
				double currentTime = CAAnimation.CurrentMediaTime ();
				timeSinceLastDraw = currentTime - timeSinceLastDrawPreviousTime;
				timeSinceLastDrawPreviousTime = currentTime;
			}

			((ImageView)View).Display ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			renderer = new Renderer ();
			var renderView = (ImageView)View;
			renderView.Renderer = renderer;

			// load all renderer assets before starting game loop
			renderer.Configure (renderView);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			didEnterBackgroundObserver.Dispose ();
			willEnterForegroundObserver.Dispose ();
		}

		public override void ViewWillAppear (bool animated)
		{
			Renderer = renderer;

			base.ViewWillAppear (animated);

			didEnterBackgroundObserver = UIApplication.Notifications.ObserveDidEnterBackground (DidEnterBackground);
			willEnterForegroundObserver = UIApplication.Notifications.ObserveWillEnterForeground (WillEnterForeground);

			// run the game loop
			DispatchGameLoop ();
		}

		void DidEnterBackground (object sender, NSNotificationEventArgs notification)
		{
			Paused = true;
		}

		void WillEnterForeground (object sender, NSNotificationEventArgs notification)
		{
			Paused = false;
		}
	}
}
