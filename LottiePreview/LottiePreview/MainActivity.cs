using System;
using Android.App;
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
    }
}

