﻿using APP.Models;
using APP.Services;
using GalaSoft.MvvmLight.Command;
using Plugin.Connectivity;
using System.ComponentModel;
using System.Windows.Input;

namespace APP.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        private ApiService apiService;

        private DialogService dialogService;

        private string email;

        private string password;

        private bool isRunning;

        private bool isEnabled;

        private bool isRemembered;
        #endregion

        #region Properties
        public string Email
        {
            set
            {
                if (email != value)
                {
                    email = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Email"));
                }
            }
            get
            {
                return email;
            }
        }

        public string Password
        {
            set
            {
                if (password != value)
                {
                    password = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Password"));
                }
            }
            get
            {
                return password;
            }
        }

        public bool IsRunning
        {
            set
            {
                if (isRunning != value)
                {
                    isRunning = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRunning"));
                }
            }
            get
            {
                return isRunning;
            }
        }

        public bool IsEnabled
        {
            set
            {
                if (isEnabled != value)
                {
                    isEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsEnabled"));
                }
            }
            get
            {
                return isEnabled;
            }
        }

        public bool IsRemembered
        {
            set
            {
                if (isRemembered != value)
                {
                    isRemembered = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRemembered"));
                }
            }
            get
            {
                return isRemembered;
            }
        }
        #endregion

        #region Constructors
        public LoginViewModel()
        {
            apiService = new ApiService();
            dialogService = new DialogService();

            IsRemembered = true;
            IsEnabled = true;
        }
        #endregion

        #region Commands
        public ICommand LoginCommand { get { return new RelayCommand(Login); } }

        private async void Login()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await dialogService.ShowMessage("Error", "Debe ingresar su email.");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await dialogService.ShowMessage("Error", "Debe ingresar su Contraseña.");
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            if (!CrossConnectivity.Current.IsConnected)
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage("Error", "Por favor, revise su conexion a internet.");
                return;
            }

            var isReachable = await CrossConnectivity.Current.IsRemoteReachable("google.com");
            if (!isReachable)
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage("Error", "Por favor, revise su conexion a internet.");
                return;
            }

            //var parameters = dataService.First<Parameter>(false);

            //var token = await apiService.GetToken(parameters.URLBase, Email, Password);
            var token = await apiService.GetToken("http://apigym.azurewebsites.net", Email, Password);

            if (token == null)
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage("Error", "El Email o la Contraseña es Incorrecto.");
                Password = null;
                return;
            }

            if (string.IsNullOrEmpty(token.AccessToken))
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage("Error", token.ErrorDescription);
                Password = null;
                return;
            }

            //var response = await apiService.GetUserByEmail(parameters.URLBase,
            var response = await apiService.GetUserByEmail("http://apigym.azurewebsites.net",
                "/api",
                "/Users/GetUserByEmail",
                token.TokenType,
                token.AccessToken,
                token.UserName);

            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage("Error", "Ha ocurrido un problema recuperando la informacion del usuario, intente mas tarde.");
                return;
            }

            //Email = null;
            //Password = null;

            IsRunning = false;
            IsEnabled = true;

            var user = (User)response.Result;
            await dialogService.ShowMessage("Biembenido", string.Format("{0} {1}", user.FirstName, user.LastName));
            //user.AccessToken = token.AccessToken;
            //user.TokenType = token.TokenType;
            //user.TokenExpires = token.Expires;
            //user.IsRemembered = IsRemembered;
            //user.Password = Password;

            //var mainViewModel = MainViewModel.GetInstance();
            //mainViewModel.CurrentUser = user;
            //navigationService.SetMainPage("MasterPage");
        }
        #endregion
    }
}
