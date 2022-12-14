﻿using Ifpa.Models;
using Microsoft.Extensions.Configuration;
using PinballApi.Models.v2.WPPR;
using PinballApi.Models.WPPR.v2.Players;
using System.Diagnostics;

namespace Ifpa.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private Player playerRecord = new Player { PlayerStats = new PinballApi.Models.WPPR.v1.Players.PlayerStats { }, ChampionshipSeries = new List<ChampionshipSeries> { } };

        public string PlayerAvatar
        {
            get
            {
                if (PlayerRecord.ProfilePhoto != null)
                    return PlayerRecord.ProfilePhoto?.ToString();
                else
                    return AppSettings.PlayerProfileNoPicUrl;
            }
        }

        public SettingsViewModel(IConfiguration config) : base(config)
        {

        }

        public async Task LoadPlayer()
        {
            try
            {
                if (Settings.MyStatsPlayerId > 0)
                {
                    IsBusy = true;
                    var playerData = await PinballRankingApiV2.GetPlayer(Settings.MyStatsPlayerId);              

                    PlayerRecord = playerData;               
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public Player PlayerRecord
        {
            get { return playerRecord; }
            set
            {
                playerRecord = value;
                OnPropertyChanged(null);
            }
        }

        public string Name => PlayerRecord.FirstName + " " + PlayerRecord.LastName;

        public bool NotifyOnRankChange
        {
            get => Settings.NotifyOnRankChange;
            set
            {
                Settings.NotifyOnRankChange = value;
                OnPropertyChanged(nameof(NotifyOnRankChange));
            }
        }
        public bool NotifyOnTournamentResult
        {
            get => Settings.NotifyOnTournamentResult;
            set
            {
                Settings.NotifyOnTournamentResult = value;
                OnPropertyChanged(nameof(NotifyOnTournamentResult));
            }
        }

        public bool NotifyOnNewBlogPost
        {
            get => Settings.NotifyOnNewBlogPost;
            set
            {
                Settings.NotifyOnNewBlogPost = value;
                OnPropertyChanged(nameof(NotifyOnNewBlogPost));
            }
        }
    }
}
