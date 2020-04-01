using Android.Content;
using Android.Widget;

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