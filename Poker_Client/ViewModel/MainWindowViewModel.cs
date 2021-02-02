using GalaSoft.MvvmLight.Command;
using Poker_Client.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Poker_Client.ViewModel
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		#region Command prefixes
		private static readonly string PRE_UsersOnline = "/online";
		private static readonly string PRE_ShowCard = "/showcard";
		private static readonly string PRE_SendCard = "/sendcard";
		#endregion
		private int idxCarta = 0;

		#region Propietats
		private string _carta;
		public string Carta
		{
			get
			{
				return _carta;
			}
			set
			{
				if (Cards.RedCards.Contains(value))
				{
					ColorCarta = "Red";
				}
				else
				{
					ColorCarta = "Black";
				}
				_carta = value;
				NotifyPropertyChanged();
			}
		}

		private string _carta1;
		public string Carta1
		{
			get
			{
				return _carta1;
			}
			set
			{
				if (Cards.RedCards.Contains(value))
				{
					ColorCarta = "Red";
				}
				else
				{
					ColorCarta = "Black";
				}
				_carta1 = value;
				NotifyPropertyChanged();
			}
		}

		private string _carta2;
		public string Carta2
		{
			get
			{
				return _carta2;
			}
			set
			{
				if (Cards.RedCards.Contains(value))
				{
					ColorCarta = "Red";
				}
				else
				{
					ColorCarta = "Black";
				}
				_carta2 = value;
				NotifyPropertyChanged();
			}
		}

		private string _carta3;
		public string Carta3
		{
			get
			{
				return _carta3;
			}
			set
			{
				if (Cards.RedCards.Contains(value))
				{
					ColorCarta = "Red";
				}
				else
				{
					ColorCarta = "Black";
				}
				_carta3 = value;
				NotifyPropertyChanged();
			}
		}

		private string _carta4;
		public string Carta4
		{
			get
			{
				return _carta4;
			}
			set
			{
				if (Cards.RedCards.Contains(value))
				{
					ColorCarta = "Red";
				}
				else
				{
					ColorCarta = "Black";
				}
				_carta4 = value;
				NotifyPropertyChanged();
			}
		}


		private string _colorCarta = "Black";
		public string ColorCarta
		{
			get
			{
				return _colorCarta;
			}
			set
			{
				_colorCarta = value;
				NotifyPropertyChanged();
			}
		}

		private string _nom;
		public string Nom
		{
			get
			{
				return _nom;
			}
			set
			{
				_nom = value;
				NotifyPropertyChanged();
			}
		}

		private ObservableCollection<string> _chatList;
		public ObservableCollection<string> ChatList
		{
			get
			{
				return _chatList;
			}
			set
			{
				_chatList = value;
				NotifyPropertyChanged();
			}
		}

		private ObservableCollection<string> _usuaris;
		public ObservableCollection<string> Usuaris
		{
			get
			{
				return _usuaris;
			}
			set
			{
				_usuaris = value;
				NotifyPropertyChanged();
			}
		}

		private string _message;
		public string Message
		{
			get
			{
				return _message;
			}
			set
			{
				_message = value;
				NotifyPropertyChanged();
			}
		}

		private string _btnName;
		public string BtnName
		{
			get
			{
				return _btnName;
			}
			set
			{
				_btnName = value;
				NotifyPropertyChanged();
			}
		}

		private string _btnColor;
		public string BtnColor
		{
			get
			{
				return _btnColor;
			}
			set
			{
				_btnColor = value;
				NotifyPropertyChanged();
			}
		}

		private bool _enabled;
		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
				NotifyPropertyChanged();
			}
		}

		public RelayCommand<string> BtnConnectCommand { get; set; }
		public RelayCommand BtnSend { get; set; }

		#endregion



		public MainWindowViewModel()
		{
			BtnName = "Connectar";
			BtnColor = "Red";
			Enabled = false;
			//ChatList = new ObservableCollection<string>();
			BtnConnectCommand = new RelayCommand<string>(ConnectDisconnect);
			BtnSend = new RelayCommand(async () => await SendMessage());
		}

		public async void ConnectDisconnect(string nom)
		{
			try
			{
				if (nom == "Connectar")
				{
					ChatList = new ObservableCollection<string>();
					BtnName = "Desconnectar";
					BtnColor = "Green";
					Enabled = true;
					await Start();
				}
				else
				{
					cts.Cancel();
					ChatList.Add("Desconnectat!"); Console.WriteLine("Desconnectat");
					BtnName = "Connectar";
					BtnColor = "Red";
					Carta = Cards.Cover;
					Enabled = false;
					Usuaris = new ObservableCollection<string>();
					return;
				}
			} catch (Exception e) { }
		}

		CancellationTokenSource cts;
		ClientWebSocket socket;
		public async Task Start()
		{
			string nom = Nom;

			cts = new CancellationTokenSource();
			socket = new ClientWebSocket();
			ChatList.Add("Connectant...");

			string wsUri = string.Format("wss://localhost:44385/api/websocket?nom={0}", nom);
			await socket.ConnectAsync(new Uri(wsUri), cts.Token);
			//ChatList.Add(socket.State.ToString());
			Console.WriteLine(socket.State.ToString());
			await Task.Factory.StartNew(
				async () =>
				{
					var rcvBytes = new byte[128];
					var rcvBuffer = new ArraySegment<byte>(rcvBytes);
					while (true)
					{
						WebSocketReceiveResult rcvResult = await socket.ReceiveAsync(rcvBuffer, cts.Token);
						byte[] msgBytes = rcvBuffer.Skip(rcvBuffer.Offset).Take(rcvResult.Count).ToArray();
						string rcvMsg = Encoding.UTF8.GetString(msgBytes);
						if (rcvMsg != null)
						{
							
							if (rcvMsg.StartsWith(PRE_UsersOnline))
							{
								rcvMsg = rcvMsg.Substring(PRE_UsersOnline.Length);
								List<string> users = rcvMsg.Split(',').ToList();
								App.Current.Dispatcher.Invoke((System.Action)delegate
								{
									Usuaris = new ObservableCollection<string>();
									foreach (string user in users)
									{
										Usuaris.Add(user);
									}
								});
							}
							else if (rcvMsg.StartsWith(PRE_ShowCard))
							{
								rcvMsg = rcvMsg.Substring(PRE_ShowCard.Length);
								MostrarCarta(rcvMsg);
							}
							else if (rcvMsg.StartsWith(PRE_SendCard)){
								rcvMsg = rcvMsg.Substring(PRE_SendCard.Length);
								RebreCarta(rcvMsg);
							}
							else
							{
								RebreMissatge(rcvMsg);
							}
						}
					}
				}, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		}

		public async Task SendMessage()
		{
			if (Message != null && Message.Length > 0)
			{
				var missatge = Message;

				byte[] sendBytes = Encoding.UTF8.GetBytes(missatge);
				var sendBuffer = new ArraySegment<byte>(sendBytes);
				await socket.SendAsync(sendBuffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: cts.Token);
				if (missatge == "Adeu")
				{
					cts.Cancel();
					ChatList.Add("Desconnectat!");
					BtnName = "Connectar";
					BtnColor = "Red";
					Message = "";
					return;
				}
				Message = "";
			}
		}

		private void RebreMissatge(string missatge)
		{
			App.Current.Dispatcher.Invoke((System.Action)delegate
			{
				ChatList.Add(missatge);
			});
		}

		private void RebreCarta(string carta)
		{
			// fer un array de strings per les cartes de l'usuari i amb l'idxCarta accedir a la que toqui
			// no tenir 5 variables diferents
		}

		private void MostrarCarta(string carta)
		{
			Carta = carta;
		}


		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
