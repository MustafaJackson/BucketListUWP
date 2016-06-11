using BucketListUWP.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;

namespace BucketListUWP.DataModel
{
    public class BucketItem : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ObservableCollection<DateTime> Dates { get; set; }

        [IgnoreDataMember]
        public ICommand CompletedCommand { get; set; }

        public BucketItem()
        {
            CompletedCommand = new CompletedButtonClick();
            Dates = new ObservableCollection<DateTime>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddDate()
        {
            Dates.Add(DateTime.Today);
            NotifyPropertyChanged("Dates");
        }

    }


    public class DataSource
    {
        private ObservableCollection<BucketItem> _bucketItems;
        const string fileName = "bucketItems.json";
        public DataSource()
        {
            _bucketItems = new ObservableCollection<BucketItem>();
        }

        public async Task<ObservableCollection<BucketItem>> GetBucketItems()
        {
            await ensureDataLoaded();
            return _bucketItems;
        }

        private async Task ensureDataLoaded()
        {
            if (_bucketItems.Count == 0)
                await getBucketItemDataAsync();
            return;
        }

        private async Task getBucketItemDataAsync()
        {
            if (_bucketItems.Count != 0)
                return;

            var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<BucketItem>));

            try
            {
                using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(fileName))
                {
                    _bucketItems = (ObservableCollection<BucketItem>)jsonSerializer.ReadObject(stream);
                }
            }

            catch
            {
                _bucketItems = new ObservableCollection<BucketItem>();
            }
        }

        public async void addItem(string name, string description)
        {
            var bucketItem = new BucketItem();
            bucketItem.Name = name;
            bucketItem.Description = description;
            bucketItem.Dates = new ObservableCollection<DateTime>();

            _bucketItems.Add(bucketItem);
            await saveBucketItemDataAsync();
        }

        private async Task saveBucketItemDataAsync()
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<BucketItem>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(fileName, CreationCollisionOption.ReplaceExisting))
            {
                jsonSerializer.WriteObject(stream, _bucketItems);
            }
        }

        public async void CompleteGoalToday(BucketItem bucketItem)
        {
            int index = _bucketItems.IndexOf(bucketItem);
            _bucketItems[index].AddDate();
            await saveBucketItemDataAsync();
        }
    }
}
