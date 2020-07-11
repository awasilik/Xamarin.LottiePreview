using System;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Com.Airbnb.Lottie;
using Google.Android.Material.Button;
using ZXing.Mobile;

namespace LottiePreview
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private LottieAnimationView animationView;
        private MaterialButton scanButton;
        private MaterialButton playButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            MobileBarcodeScanner.Initialize(Application);
            SetContentView(Resource.Layout.activity_main);

            animationView = FindViewById<LottieAnimationView>(Resource.Id.animation);

            scanButton = FindViewById<MaterialButton>(Resource.Id.button);
            scanButton.Click += OnScanButtonClicked;

            playButton = FindViewById<MaterialButton>(Resource.Id.playbutton);
            playButton.Click += OnPlayButtonClicked;

            var background = FindViewById(Resource.Id.background);

            var whiteButton = FindViewById(Resource.Id.color_button_white);
            whiteButton.Click += (s, a) => background.SetBackgroundResource(Resource.Drawable.color_background_white);
            var blackButton = FindViewById(Resource.Id.color_button_black);
            blackButton.Click += (s, a) => background.SetBackgroundResource(Resource.Drawable.color_background_black);
            var transparentButton = FindViewById(Resource.Id.color_button_transparent);
            transparentButton.Click += (s, a) => background.SetBackgroundResource(Resource.Drawable.color_background_transparent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private async void OnScanButtonClicked(object sender, EventArgs e)
        {
            var scanner = new MobileBarcodeScanner();

            var result = await scanner.Scan();

            try
            {
                if (!result.Text.EndsWith("json"))
                {
                    throw new InvalidOperationException();
                }

                animationView.ClearAnimation();
                animationView.SetAnimationFromUrl(result.Text);
                animationView.PlayAnimation();
            }
            catch
            {
                Toast.MakeText(this, "Not a valid QR code", ToastLength.Long).Show();
            }
        }
        private async void OnPlayButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (animationView == null)
                {
                    throw new InvalidOperationException();
                }
                animationView.PlayAnimation();
            }
            catch
            {
                Toast.MakeText(this, "No animation", ToastLength.Long).Show();
            }
        }
    }
}

