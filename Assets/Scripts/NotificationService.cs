using System;
using UnityEngine;

#if UNITY_ANDROID
using Unity.Notifications.Android;
using UnityEngine.Android;
#endif

#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

public sealed class NotificationService
{
    private const string AndroidChannelId = "daily_reminder";
    private const string AndroidChannelName = "Daily Reminders";
    private const string AndroidChannelDesc = "Daily game reminders";

    private const string IOSNotificationId = "daily_reminder";

    public void Initialize()
    {
#if UNITY_ANDROID
        RegisterAndroidChannel();
#endif
    }

    public void ApplySettings(bool enabled, int hour, int minute)
    {
        CancelAll();

        if (!enabled)
            return;

#if UNITY_ANDROID
        RequestAndroidPermission();
        ScheduleAndroid(hour, minute);
#endif

#if UNITY_IOS
        ScheduleIOS(hour, minute);
#endif
    }

    public void CancelAll()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllScheduledNotifications();
#endif

#if UNITY_IOS
        iOSNotificationCenter.RemoveAllScheduledNotifications();
#endif
    }

#if UNITY_ANDROID
    private void RegisterAndroidChannel()
    {
        var channel = new AndroidNotificationChannel
        {
            Id = AndroidChannelId,
            Name = AndroidChannelName,
            Importance = Importance.Default,
            Description = AndroidChannelDesc
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    private void RequestAndroidPermission()
    {
        if (Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            return;

        Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
    }

    private void ScheduleAndroid(int hour, int minute)
    {
        var notification = new AndroidNotification
        {
            Title = "Sky Adventure",
            Text = "Забери щоденний бонус ✈️",
            FireTime = GetNextTime(hour, minute),
            RepeatInterval = TimeSpan.FromDays(1)
        };

        AndroidNotificationCenter.SendNotification(
            notification,
            AndroidChannelId
        );
    }
#endif

#if UNITY_IOS
    private void ScheduleIOS(int hour, int minute)
    {
        var trigger = new iOSNotificationCalendarTrigger
        {
            Hour = hour,
            Minute = minute,
            Repeats = true
        };

        var notification = new iOSNotification
        {
            Identifier = IOSNotificationId,
            Title = "Sky Adventure",
            Body = "Забери щоденний бонус ✈️",
            ShowInForeground = true,
            ForegroundPresentationOption =
                PresentationOption.Alert |
                PresentationOption.Sound,
            Trigger = trigger
        };

        iOSNotificationCenter.ScheduleNotification(notification);
    }
#endif

    private DateTime GetNextTime(int hour, int minute)
    {
        var now = DateTime.Now;
        var target = new DateTime(
            now.Year,
            now.Month,
            now.Day,
            hour,
            minute,
            0
        );

        if (target <= now)
            target = target.AddDays(1);

        return target;
    }
}
