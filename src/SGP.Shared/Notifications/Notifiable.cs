﻿using FluentValidation.Results;
using System.Collections.Generic;

namespace SGP.Shared.Notifications
{
    public abstract class Notifiable
    {
        private readonly List<Notification> _notifications;

        protected Notifiable()
        {
            _notifications = new List<Notification>();
        }

        public bool IsValid => _notifications.Count == 0;
        public IReadOnlyList<Notification> Notifications => _notifications;

        public void AddNotification(string key, string message)
        {
            _notifications.Add(new Notification(key, message));
        }

        public void AddNotification(Notification notification)
        {
            _notifications.Add(notification);
        }

        public void AddNotifications(IReadOnlyCollection<Notification> notifications)
        {
            _notifications.AddRange(notifications);
        }

        public void AddNotifications(IList<Notification> notifications)
        {
            _notifications.AddRange(notifications);
        }

        public void AddNotifications(ICollection<Notification> notifications)
        {
            _notifications.AddRange(notifications);
        }

        public void AddNotifications(Notifiable notifiable)
        {
            AddNotifications(notifiable?.Notifications);
        }

        public void AddNotifications(params Notifiable[] notifiables)
        {
            foreach (var item in notifiables)
            {
                AddNotifications(item);
            }
        }

        public void AddNotifications(ValidationResult result)
        {
            if (result?.IsValid == false)
            {
                foreach (var failure in result.Errors)
                {
                    AddNotification(failure.PropertyName, failure.ErrorMessage);
                }
            }
        }

        public void Clear()
        {
            _notifications.Clear();
        }
    }
}