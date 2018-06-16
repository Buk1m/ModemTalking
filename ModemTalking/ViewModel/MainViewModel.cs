using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using ModemTalking.Commands;
using ModemTalking.Model;

namespace ModemTalking.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        #region Private Members

        private Model.ModemTalking _modemTalking;
        private string _selectedCom;
        private string _senderMessage;
        private string _reciverMessage;
        private int _selectedBaudrate = 9600;

        #endregion

        #region Constructor

        public MainViewModel()
        {
            OpenFile = new RelayCommand(() =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Text files (*.txt)|*.txt" };
                if (openFileDialog.ShowDialog() != true)
                    return;

                SenderTextBox = File.ReadAllText(openFileDialog.FileName);
            });

            Send = new RelayCommand(() =>
            {
                if (_modemTalking == null || _selectedCom == null)
                {
                    MessageBox.Show("Choose serial port.");
                    return;
                }

                if (SenderTextBox == null)
                {
                    MessageBox.Show("Enter message to be sent.");
                    return;
                }

                _modemTalking.Send(SenderTextBox);
            });

            RefreshPorts = new RelayCommand(() =>
            {
                _modemTalking?.Close();
                _selectedCom = null;
                ComPorts?.Clear();
                foreach (var portName in SerialPort.GetPortNames())
                {
                    ComPorts?.Add(portName);
                }
            });

            ClosePorts = new RelayCommand(() => { _modemTalking?.Close(); });
        }

        #endregion

        #region Public Properties

        public ObservableCollection<string> ComPorts { get; set; } =
            new ObservableCollection<string>(SerialPort.GetPortNames());

        public ObservableCollection<string> Baudrate { get; set; } =
            new ObservableCollection<string>() { "9600", "115200" };

        public string SelectedSenderBaudrate
        {
            get => _selectedBaudrate.ToString();
            set
            {
                _selectedBaudrate = int.Parse(value);
                OnPropertyChanged();
            }
        }

        private void Recive(object sender, EventArgs e)
        {
            ReciverTextBox = _modemTalking.Read();
        }

        public string SelectedSenderCom
        {
            get => _selectedCom;
            set
            {
                if (_selectedCom == value || value == null)
                    return;

                _selectedCom = value;
                try
                {
                    _modemTalking?.Close();
                    _modemTalking = new Model.ModemTalking(_selectedCom, _selectedBaudrate);
                    _modemTalking.MyHeart.DataReceived += Recive;
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Port is being used by other process. Acces denied.");
                    OnPropertyChanged();
                }

                OnPropertyChanged();
            }
        }

        public string SenderTextBox
        {
            get => _senderMessage;

            set
            {
                _senderMessage = value;
                OnPropertyChanged();
            }
        }

        public string ReciverTextBox
        {
            get => _reciverMessage;

            set
            {
                _reciverMessage += value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public ICommand OpenFile { get; set; }
        public ICommand Send { get; set; }
        public ICommand RefreshPorts { get; set; }
        public ICommand ClosePorts { get; set; }

        #endregion
    }

}
