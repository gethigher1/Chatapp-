using ChatClient.MVVM.Core;
using ChatClient.MVVM.Model;
using ChatClient.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows;

namespace ChatClient.MVVM.ViewModel
{
    class MainViewModel
    {

        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }

        public RelayCommand ConnectToServerCommand { get; set; }//property
        public RelayCommand SendMessageCommand { get; set; }//command

        public string Username { get; set; }
        public string Message { get; set; }


        private server server;
        public MainViewModel()//konstruktor
        {
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();
            server = new server();
            server.connectedEvent += UserConnected;
            server.msgRecievedEvent += MessageRecieved;
            server.userDisconnectEvent += RemoveUser;
            ConnectToServerCommand = new RelayCommand(o => server.ConnectToServer(Username), o => !string.IsNullOrEmpty(Username));
            //relay -> ha nincs mező nem csatlakozik ||N2M -> csinald meg kesobb a 2 utas adat bindingot

            SendMessageCommand = new RelayCommand(o => server.SendMessageToServer(Message), o => !string.IsNullOrEmpty(Message));
                
        }
        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = server.PacketReader.ReadMessage(),
                UID = server.PacketReader.ReadMessage(),
            };
            if (!Users.Any(x => x.UID == user.UID))
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }

        private void RemoveUser()
        {
            var uid = server.PacketReader.ReadMessage();
            var user = Users.Where(x => x.UID == uid).FirstOrDefault();
            System.Windows.Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }

        private void MessageRecieved()
        {
            //read data
            var msg = server.PacketReader.ReadMessage();
            System.Windows.Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
        }
    }
}
