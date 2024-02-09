using ExchangeRates.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Telerik.Charting;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ChartView;
using System.Linq;
using System.Collections.Generic;

namespace ExchangeRates
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CurrencyRatesGrid.PreviewMouseLeftButtonUp += GridMouseLeftButtonUp;
        }

        private async void GetCurrenciesRates(object sender, RoutedEventArgs e)
        {
            if (StartDate.SelectedValue.HasValue && EndDate.SelectedValue.HasValue)
            {
                var startDate = new DateTime(StartDate.SelectedValue.Value.Year, StartDate.SelectedValue.Value.Month, StartDate.SelectedValue.Value.Day);
                var endDate = new DateTime(EndDate.SelectedValue.Value.Year, EndDate.SelectedValue.Value.Month, EndDate.SelectedValue.Value.Day);

                var collection = await CurrencyRateService.GetCurrencyRatesByPeriod(startDate, endDate);
                var context = (MyViewModel)CurrencyRatesGrid.DataContext;
                context.SetUpCollection(collection);
            }
        }

        private void CellHandler(object sender, Telerik.Windows.Controls.GridViewCellEditEndedEventArgs e)
        {
            var context = (MyViewModel)CurrencyRatesGrid.DataContext;
            CurrencyRateService.SaveChangesInCollection(context.CurrencyRates);
        }

        private void GridMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var selectdeGrid = (RadGridView)e.Source;
            var selectedItem = (CurrencyRate)selectdeGrid.SelectedItem;
            
            if (selectedItem != null)
            {
                var itemsList = selectdeGrid.Items.SourceCollection as IList<CurrencyRate>;
                var itemsSetForChart = itemsList.Where(i => i.Abbreviation == selectedItem.Abbreviation)?.ToList();

                ChartRender(itemsSetForChart, selectedItem);
            }
        }

        private void ChartRender(List<CurrencyRate> rates, CurrencyRate selectedItem)
        {
            CurrencyAbb.Text = selectedItem.Abbreviation;

            this.LayoutRoot.Children.Clear();

            RadCartesianChart chart = new RadCartesianChart();
            chart.HorizontalAxis = new CategoricalAxis();
            chart.VerticalAxis = new LinearAxis() { Maximum = 14 };
            LineSeries line = new LineSeries();
            line.Stroke = new SolidColorBrush(Colors.Orange);

            foreach (var item in rates)
            {
                line.DataPoints.Add(new CategoricalDataPoint() { Value = item.OfficialRate <= 10 ? item.OfficialRate : 14 });
            }

            chart.Series.Add(line);
            chart.SetValue(Grid.ColumnProperty, 0);

            this.LayoutRoot.Children.Add(chart);
        }
    }
}
