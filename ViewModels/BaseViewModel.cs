using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Ifpa.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PinballApi;
using PropertyChanged;

namespace Ifpa.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public PinballRankingApiV1 PinballRankingApi { get; private set; } 
        public PinballRankingApiV2 PinballRankingApiV2 { get; private set; }

        protected readonly ILogger logger;

        public bool IsBusy { get; set; }      

        public string Title { get; set; }      

        public BaseViewModel(PinballRankingApiV1 pinballRankingApiV1, PinballRankingApiV2 pinballRankingApiV2, ILogger<BaseViewModel> logger)
        {         
            PinballRankingApi = pinballRankingApiV1;
            PinballRankingApiV2 = pinballRankingApiV2;
            this.logger = logger;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
