using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALTCG_bot
{
    public enum CardType
    {
        Member, Effect
    }

    public enum Rarity
    {
        Common, Rare, Epic, Legendary
    }

    public enum MemberType
    {
        Null,
        Connecte,
        Invisible,
        Rageux,
        Weeb,
        Fetichiste,
        Vanilla
    }

    internal class Card
    {
        protected int _number;
        protected string _name;
        protected Rarity _rarity;
        protected CardType _cardType;
        protected AEffect _effect;
        protected string _visualPath;

        public string Name { get { return _name; } }
        public Rarity Rarity { get { return _rarity; } }
        public CardType CardType { get { return _cardType; } }
        public string Path { get { return _visualPath; } }

        public Card(int number, string name, CardType cardType, Rarity rarity, AEffect effect = null)
        {
            _number = 0;
        }

        public Card()
        {

        }
    }

    internal class MemberCard : Card
    {
        private int _force;
        private MemberType _memberType;
        private List<MemberType> _advantageType;
        private List<MemberType> _disadvantageType;

        public int Force { get { return _force; }}
        public MemberType MemberType { get { return _memberType; } }
        public List<MemberType> AdvantageType { get { return _advantageType; } }
        public List<MemberType> DisadvantageType { get { return _disadvantageType; } }

        public MemberCard(int number, string name, CardType cardType, Rarity rarity, 
            int force, MemberType memberType, MemberType advantageType1 = MemberType.Null, MemberType advantageType2 = MemberType.Null, 
            MemberType disadvantageType1 = MemberType.Null, MemberType disadvantageType2 = MemberType.Null, AEffect effect = null)
            : base(number, name, cardType, rarity, effect)
        {
            _force = force;
            _memberType = memberType;
            _advantageType = new List<MemberType>();
            _disadvantageType = new List<MemberType>();

            if (advantageType1 != MemberType.Null)
            {
                _advantageType.Add(advantageType1);
            }

            if (advantageType2 != MemberType.Null)
            {
                _advantageType.Add(advantageType2);
            }

            if (disadvantageType1 != MemberType.Null)
            {
               _disadvantageType.Add(disadvantageType1);
            }

            if (disadvantageType2 != MemberType.Null)
            {
                _disadvantageType.Add(disadvantageType2);
            }
        }

        public MemberCard(int force, string path)
        {
            _number = 0;
            _name = "TempTestCard";
            _rarity = Rarity.Common;
            _effect = null;
            _force = force;
            _memberType = MemberType.Vanilla;
            _visualPath = path;
        }
    }
}
