﻿using Ifpa.ViewModels;
using PinballApi.Models.WPPR.v2.Series;

namespace Ifpa.Views
{
    [QueryProperty("SeriesCode", "seriesCode")]
    [QueryProperty("RegionCode", "regionCode")]
    [QueryProperty("Year", "year")]
    [QueryProperty("PlayerId", "playerId")]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChampionshipSeriesPlayerCardPage : ContentPage
    {
        ChampionshipSeriesPlayerCardViewModel ViewModel;

        public int Year { get; set; }
        public int PlayerId { get; set; }
        public string RegionCode { get; set; }
        public string SeriesCode { get; set; }

        public ChampionshipSeriesPlayerCardPage(ChampionshipSeriesPlayerCardViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.ViewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();        

            if (ViewModel.TournamentCardRecords?.Count == 0)
            {
                ViewModel.PlayerId = PlayerId;
                ViewModel.RegionCode = RegionCode;
                ViewModel.SeriesCode = SeriesCode;     
                ViewModel.Year = Year;

                ViewModel.LoadItems();
            }
        }
    }
}