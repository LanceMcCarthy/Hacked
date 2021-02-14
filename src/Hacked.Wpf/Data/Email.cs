using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Hacked.Wpf.Helpers;

namespace Hacked.Wpf.Data
{
    public class Email : INotifyPropertyChanged, ICloneable
    {
        private string _carbonCopy;
        private string _content;
        private DateTime _received;
        private string _recipient;
        private string _sender;
        private Enums.EmailStatus _status;
        private string _subject;

        public Email()
        { }

        public Email(string sender, string recipient, string subject, DateTime received)
        {
            Sender = sender;
            Recipient = recipient;
            Subject = subject;
            Received = received;
        }

        /// <summary>
        /// Gets or sets the CarbonCopy of the email.
        /// </summary>
        public string CarbonCopy
        {
            get => _carbonCopy;
            set
            {
                if (_carbonCopy != value)
                {
                    _carbonCopy = value;
                    OnPropertyChanged("CarbonCopy");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Content of the email.
        /// </summary>
        public string Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                    OnPropertyChanged("Content");
                }
            }
        }

        /// <summary>
        /// Gets or sets date the email has been received.
        /// </summary>
        public DateTime Received
        {
            get => _received;
            set
            {
                if (_received != value)
                {
                    _received = value;
                    OnPropertyChanged("Received");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Recipient address of the email.
        /// </summary>
        public string Recipient
        {
            get => _recipient;
            set
            {
                if (_recipient != value)
                {
                    _recipient = value;
                    OnPropertyChanged("Recipient");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Sender address of the email.
        /// </summary>
        public string Sender
        {
            get => _sender;
            set
            {
                if (_sender != value)
                {
                    _sender = value;
                    OnPropertyChanged("Sender");
                }
            }
        }

        /// <summary>
        /// Gets or sets Status of the Email object.
        /// </summary>
        public Enums.EmailStatus Status
        {
            get => _status;

            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Subject of the email.
        /// </summary>
        public string Subject
        {
            get => _subject;
            set
            {
                if (_subject != value)
                {
                    _subject = value;
                    OnPropertyChanged("Subject");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public object Clone()
        {
            var otherEmail = new Email();

            SetPropertyValues(otherEmail);

            return otherEmail;
        }

        private void SetPropertyValues(Email otherEmail)
        {
            var propertyInfo = GetType().GetProperties().Where(p => p.CanWrite && (p.PropertyType.IsValueType || p.PropertyType.IsEnum || p.PropertyType.Equals(typeof(String))));

            foreach (PropertyInfo property in propertyInfo)
            {
                if (property.CanWrite)
                {
                    property.SetValue(otherEmail, property.GetValue(this, null), null);
                }
            }
        }
    }
}