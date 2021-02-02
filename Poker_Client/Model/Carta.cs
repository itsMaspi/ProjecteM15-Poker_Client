using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker_Client.Model
{
	public class Carta
	{
		public string valor { get; set; }
		public string color { get; set; }

		public Carta()
		{

		}
		public Carta(string valor)
		{
			this.valor = valor;
		}
	}
}
