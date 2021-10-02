using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matcher.Classes
{
    public class Player
    {
        private string firstName;
        private string lastName;
        private string faction;

        public List<Player> opponents;

        public Player(string fn, string ln, string f)
        {
            firstName = fn;
            lastName = ln;
            faction = f;
        }

        public void SetOpponents(int op)
        {
            opponents = new List<Player>(op);
        }

        public bool TryAssigning(Player p)
        {
            if (!opponents.Contains(p) && opponents.Count < opponents.Capacity)
            {
                opponents.Add(p);
                return true;
            }
            else return false;
        }

        public string Print()
        {
            return $"{firstName} {lastName} ({faction})";
        }

        public string PrintWithOpponents()
        {
            string s = $"{firstName} {lastName} ({faction})\n";
            int count = 1;
            foreach(var p in opponents)
            {
                s += $"\t\tRound #{count} - {p.Print()}\n";
                count++;
            }
            s += "\n";
            return s;
        }
    }
}
