using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Hacked.Forms.Portable.Controls.SimpleListView
{
    public class CollectionChangedHandle<TSyncType, T> : IDisposable where T : class where TSyncType : class
    {
        private readonly Func<T, TSyncType> _projector;
        private readonly Action<TSyncType, T, int> _postAdd;
        private readonly Action<TSyncType> _cleanup;
        private readonly INotifyCollectionChanged _itemsSourceCollectionChangedImplementation;
        private readonly IEnumerable<T> _sourceCollection;
        private readonly IList<TSyncType> _target;
        
        public CollectionChangedHandle(IList<TSyncType> target, IEnumerable<T> source, Func<T, TSyncType> projector, Action<TSyncType, T, int> postAdd = null, Action<TSyncType> cleanup = null)
        {
            if (source == null)
                return;

            _itemsSourceCollectionChangedImplementation = source as INotifyCollectionChanged;

            _sourceCollection = source;
            _target = target;
            _projector = projector;
            _postAdd = postAdd;
            _cleanup = cleanup;

            InitialPopulation();

            if (_itemsSourceCollectionChangedImplementation == null)
                return;

            _itemsSourceCollectionChangedImplementation.CollectionChanged += CollectionChanged;
        }
        
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                SafeClearTarget();
            }
            else
            {
                var items = new List<T>(_sourceCollection);

                if (args.OldItems != null)
                {
                    var syncItem = _target[args.OldStartingIndex];

                    if (syncItem != null)
                        _cleanup?.Invoke(syncItem);

                    _target.RemoveAt(args.OldStartingIndex);
                }

                if (args.NewItems != null)
                {
                    foreach (var obj in args.NewItems)
                    {
                        if (!(obj is T item))
                            continue;

                        var index = items.IndexOf(item);
                        var newSyncItem = _projector(item);

                        _target.Insert(index, newSyncItem);
                        _postAdd?.Invoke(newSyncItem, item, index);
                    }
                }
            }
        }
        
        private void InitialPopulation()
        {
            SafeClearTarget();

            foreach (var t in _sourceCollection.Where(x => x != null))
            {
                _target.Add(_projector(t));
            }
        }

        private void SafeClearTarget()
        {
            while (_target.Count > 0)
            {
                var syncType = _target[0];

                _target.RemoveAt(0);

                _cleanup?.Invoke(syncType);
            }
        }
        
        public void Dispose()
        {
            if (_itemsSourceCollectionChangedImplementation == null)
                return;

            _itemsSourceCollectionChangedImplementation.CollectionChanged -= CollectionChanged;
        }
    }
}
