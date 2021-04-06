using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Hacked.Wpf.Helpers;
using Telerik.Windows.Data;

namespace Hacked.Wpf.Data
{
    public class Folder : INotifyPropertyChanged
    {
        private QueryableCollectionView _emails;
        private IEnumerable<Folder> _folders;
        private string _name;
        private int _unreadEmailsCount = -1;

        public Folder()
        {
            Folders = new List<Folder>();
        }

        /// <summary>
        /// Gets or sets Folders and notifies for changes
        /// </summary>
        public IEnumerable<Folder> Folders
        {
            get => _folders;

            set
            {
                if (_folders != value)
                {
                    _folders = value;
                    OnPropertyChanged("Folders");
                }
            }
        }

        /// <summary>
        /// Gets or sets Name and notifies for changes
        /// </summary>
        public string Name
        {
            get => _name;

            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets or sets Emails and notifies for changes
        /// </summary>
        public QueryableCollectionView Emails
        {
            get => _emails;

            set
            {
                if (_emails != value)
                {
                    _emails = value;
                    OnPropertyChanged("Emails");
                }
            }
        }

        /// <summary>
        /// Gets the number of unread Email objects of the Folder.
        /// </summary>
        public int UnreadEmailsCount
        {
            get
            {
                if (_unreadEmailsCount == -1)
                {
                    _unreadEmailsCount = GetUnreadEmailsCount();
                }
                return _unreadEmailsCount;
            }
            private set
            {
                if (_unreadEmailsCount != value)
                {
                    _unreadEmailsCount = value;
                    OnPropertyChanged("UnreadEmailsCount");
                }
            }
        }

        /// <summary>
        /// Updates the count of the unread email object of the Folder object.
        /// </summary>
        public void UpdateUnreadEmailsCount()
        {
            UnreadEmailsCount = GetUnreadEmailsCount();
        }

        private int GetUnreadEmailsCount()
        {
            if (Emails != null)
            {
                var source = Emails.SourceCollection as IEnumerable<Email>;
                if (source != null)
                {
                    return source.Count(i => i.Status == Enums.EmailStatus.Unread);
                }
            }
            return 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}