using System;
using System.IO.Ports;
using System.Text;
using System.Windows;

namespace ModemTalking.Model
{
    /// <summary>
    /// Model do komunikacji za pomoca modemów
    /// </summary>
    class ModemTalking
    {
        // zmienna typu SeialPort do obslugi portu szeregowego
        internal SerialPort MyHeart;

        /// <summary>
        /// Konstruktor ModemTalking
        /// </summary>
        /// <param name="portName"> nazwa portu ktory ma zaostac otwarty </param>
        /// <param name="baudrate"> baudrate - predkosc komunikacji </param>
        public ModemTalking(string portName, int baudrate)
        {
            // ustawienia portu
            MyHeart = new SerialPort
            {
                // nazwa portu
                PortName = portName,
                // predkosc komuniacji
                BaudRate = baudrate,
                // timeout dla odczytu z portu
                ReadTimeout = 500,
                // timeout dla nadawania przez port
                WriteTimeout = 500,
                // ustawia sygnal Data Terminal Ready
                DtrEnable = true,
                // wskazuje czy wlaczony jest sygnal Request to send
                RtsEnable = true,
                // liczba bitów w bajcie
                DataBits = 8,
                // typ inicjalizacji polaczenia
                Handshake = Handshake.None,
                // bit parzystosci
                Parity = Parity.None,
                // bit stopu
                StopBits = StopBits.One,
                // znak nowej linii dla WriteLine i ReadLine
                NewLine = "\r"
            };

            //Otwarcie portu
            MyHeart.Open();
        }

        /// <summary>
        /// Funkcja wywylajaca dane przez port
        /// </summary>
        /// <param name="message"> wiadomosc do przeslania </param>
        public void Send(string message)
        {
            // wpisz linie z wiadomoscia
            MyHeart.WriteLine(message);
        }

        /// <summary>
        /// Funkcja odczytujaca zbuferowane dane z portu
        /// </summary>
        /// <returns></returns>
        public string Read()
        {
            try
            {
                // sprawdzenie liczby bajtów w buforze i utworzenie tablicy o takim rozmiarze
                byte[] packOfBytes = new byte[MyHeart.BytesToRead];
                // odczytanie oczekujacych w buforze bajtów
                _mySoul = Encoding.ASCII.GetString(packOfBytes, 0, MyHeart.Read(packOfBytes, 0, packOfBytes.Length));
            }
            catch (TimeoutException)
            {
                // jezeli nastapi timeout to poinformuj uzytkownika
                MessageBox.Show("Timeout");
            }

            // zwroc wiadomosc
            return _mySoul;
        }


        /// <summary>
        /// Metoda zamykajaca port i zwalniajaca zasoby
        /// </summary>
        public void Close()
        {
            // zwolnienie zasobow
            MyHeart.Dispose();
        }

        // zmienna przechowywujaca odebana wiadomosc
        private string _mySoul = "";

    }

}