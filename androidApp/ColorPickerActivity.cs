using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace androidApp
{
    [Activity(Label = "ColorPicker")]
    public class ColorPickerActivity : Activity
    {
        private Dictionary<int, TextView> textViews;
        private Android.Graphics.Color finalColor;
        private TextView colorValue;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ColorPicker);

            // Create your application here
            var redSeekBar = FindViewById<SeekBar>(Resource.Id.redSeekBar);
            var greenSeekBar = FindViewById<SeekBar>(Resource.Id.greenSeekBar);
            var blueSeekBar = FindViewById<SeekBar>(Resource.Id.blueSeekBar);

            var redTextView = FindViewById<TextView>(Resource.Id.redValue);
            var greenTextView = FindViewById<TextView>(Resource.Id.greenValue);
            var blueTextView = FindViewById<TextView>(Resource.Id.blueValue);
            colorValue = FindViewById<TextView>(Resource.Id.colorValue);

            finalColor = new Android.Graphics.Color(0, 0, 0);
            
            textViews = new Dictionary<int, TextView>();
            textViews.Add(redSeekBar.Id, redTextView);
            textViews.Add(greenSeekBar.Id, greenTextView);
            textViews.Add(blueSeekBar.Id, blueTextView);

            redSeekBar.ProgressChanged += SeekBar_ProgressChanged;
            greenSeekBar.ProgressChanged += SeekBar_ProgressChanged;
            blueSeekBar.ProgressChanged += SeekBar_ProgressChanged;

            var returnColorButton = FindViewById<Button>(Resource.Id.returnButton);
            returnColorButton.Click += delegate
            {
                var intent = new Intent(this, typeof(MainActivity));
                intent.PutExtra("colorSelected", this.GetHexValue());
                SetResult(Result.Ok, intent);
                Finish();
            };
        }

        private void SeekBar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            var seekBar = (SeekBar)sender;
            var textViewChanged = textViews[seekBar.Id];
            textViewChanged.Text = seekBar.Progress.ToString();
            Android.Graphics.Color backgroundColor; // = new Android.Graphics.Color();
            switch (seekBar.Id)
            {
                case Resource.Id.redSeekBar:
                    backgroundColor = new Android.Graphics.Color(seekBar.Progress, 0, 0);
                    finalColor.R = (byte)seekBar.Progress;
                    break;
                case Resource.Id.greenSeekBar:
                    backgroundColor = new Android.Graphics.Color(0, seekBar.Progress, 0);
                    finalColor.G = (byte)seekBar.Progress;
                    break;
                case Resource.Id.blueSeekBar:
                    backgroundColor = new Android.Graphics.Color(0, 0, seekBar.Progress);
                    finalColor.B = (byte)seekBar.Progress;
                    break;
                default:
                    backgroundColor = new Android.Graphics.Color(255, 255, 255);
                    break;
            }

            textViewChanged.SetBackgroundColor(backgroundColor);
            colorValue.SetBackgroundColor(finalColor);
            colorValue.Text = GetHexValue();
        }

        private string GetHexValue()
        {
            return string.Format("#{0}{1}{2}", 
                finalColor.R.ToString("X2"), 
                finalColor.G.ToString("X2"), 
                finalColor.B.ToString("X2"));
        }
    }
}