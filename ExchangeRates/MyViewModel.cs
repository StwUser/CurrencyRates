using ExchangeRates.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ExchangeRates
{
    public class MyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<CurrencyRate> currencyRates;
        private object selectedItem;

        public ObservableCollection<CurrencyRate> CurrencyRates
        {
            get
            {
                if (this.currencyRates == null)
                {
                    this.currencyRates = CurrencyRate.GetCurrencyRates();
                }

                return this.currencyRates;
            }
            set
            {
                if (this.currencyRates != value) 
                { 
                    this.currencyRates = value;
                }
            }
        }

        public object SelectedItem
        {
            get { return this.selectedItem; }
            set
            {
                if (value != this.selectedItem)
                {
                    this.selectedItem = value;
                    this.OnPropertyChanged("SelectedItem");
                }
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public void SetUpCollection(List<CurrencyRate> currencyRates)
        {
            this.currencyRates.Clear();
            foreach (var item in currencyRates)
            {
                this.currencyRates.Add(item);
            }
        }
    }
}
