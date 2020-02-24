using Android;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Support.V4.App;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Text;

namespace REICTEventScheduler.Services
{
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "Alert!", ToastLength.Long).Show();
        }
    }
}