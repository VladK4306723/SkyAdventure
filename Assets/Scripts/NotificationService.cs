using System;
using UnityEngine;

#if UNITY_ANDROID
using Unity.Notifications.Android;
using UnityEngine.Android;
#endif

public sealed class NotificationService
{
#if UNITY_ANDROID
    private const string AndroidChannelId = "daily_reminder";
    private const string AndroidPostNotificationsPermission =
        "android.permission.POST_NOTIFICATIONS";
#endif

    public void Initialize()
    {
#if UNITY_ANDROID
        RegisterAndroidChannel();
#endif
    }

    public void RequestPermissionFromUser()
    {
#if UNITY_ANDROID
        if (AndroidSdkInt < 33)
            return;

        if (Permission.HasUserAuthorizedPermission(AndroidPostNotificationsPermission))
            return;

        Permission.RequestUserPermission(AndroidPostNotificationsPermission);
#endif
    }

    public void ApplySettings(bool enabled, int hour, int minute)
    {
#if UNITY_ANDROID
        CancelAll();

        if (!enabled)
            return;

        if (!HasAndroidNotificationPermission())
            return;

        ScheduleAndroid(hour, minute);
#endif
    }

    public void CancelAll()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllScheduledNotifications();
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
#endif
    }

#if UNITY_ANDROID
    private void RegisterAndroidChannel()
    {
        var channel = new AndroidNotificationChannel
        {
            Id = AndroidChannelId,
            Name = "Daily Reminders",
            Importance = Importance.High,
            Description = "Daily game reminders"
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    private bool HasAndroidNotificationPermission()
    {
        if (AndroidSdkInt < 33)
            return true;

        return Permission.HasUserAuthorizedPermission(AndroidPostNotificationsPermission);
    }

    private void ScheduleAndroid(int hour, int minute)
    {
        var notification = new AndroidNotification
        {
            Title = "Sky Adventure",
            Text = "Wanna fly? ✈️",
            FireTime = GetNextTime(hour, minute),
            RepeatInterval = TimeSpan.FromDays(1)
        };

        AndroidNotificationCenter.SendNotification(notification, AndroidChannelId);
    }
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
    private static int AndroidSdkInt
    {
        get
        {
            using var version = new AndroidJavaClass("android.os.Build$VERSION");
            return version.GetStatic<int>("SDK_INT");
        }
    }
#else
    private static int AndroidSdkInt => 0;
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
