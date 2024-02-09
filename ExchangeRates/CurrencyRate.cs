using ExchangeRates.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRates
{
    public class CurrencyRate : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DateTime date;
        private string abbreviation;
        private string name;
        private double officialRate;

        public DateTime Date
        {
            get { return this.date; }
            set
            {
                if (value != this.date)
                {
                    this.date = value;
                    this.OnPropertyChanged("Date");
                }
            }
        }

        public string Abbreviation
        {
            get { return this.abbreviation; }
            set
            {
                if (value != this.abbreviation)
                {
                    this.abbreviation = value;
                    this.OnPropertyChanged("Abbreviation");
                }
            }
        }

        public string Name
        {
            get { return this.name; }
            set
            {
                if (value != this.name)
                {
                    this.name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }


        public double OfficialRate
        {
            get { return this.officialRate; }
            set
            {
                if (value != this.officialRate)
                {
                    this.officialRate = value;
                    this.OnPropertyChanged("OfficialRate");
                }
            }
        }

        public CurrencyRate()
        {

        }

        public CurrencyRate(DateTime date, string abbreviation, string name, double officialRate)
        {
            this.date = date;
            this.abbreviation = abbreviation;
            this.name = name;
            this.officialRate = officialRate;
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

        public override string ToString()
        {
            return this.Name;
        }

        public static ObservableCollection<CurrencyRate> GetCurrencyRates()
        {
            ObservableCollection<CurrencyRate> currencyRates = new ObservableCollection<CurrencyRate>();
            CurrencyRateService.FillCurrencyRatesFromJson(currencyRates);

            return currencyRates;
        }
    }
}
