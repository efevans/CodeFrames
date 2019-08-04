using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CodeFrames
{
    public class Game
    {
        private int CardCount => (2 * CardsPerTeam) + NeutralCards + DeathCards + ExtraCardsForStartingTeam;
        private const int CardsPerTeam = 8;
        private const int NeutralCards = 7;
        private const int DeathCards = 1;
        private const int ExtraCardsForStartingTeam = 1;

        public Team CurrentTeam { get; private set; }

        public int RemainingBlueCards { get; private set; }
        public int RemainingRedCards { get; private set; }

        public Frame[] Frames { get; private set; }
        public bool IsOver { get; private set; }
        public Team Winner { get; private set; }

        private IFrameValueGetter FrameValueGetter { get; }

        public Game(IFrameValueGetter fvGetter)
        {
            FrameValueGetter = fvGetter;
            StartNewGame();
        }

        public void Guess(int frameInd)
        {
            if (!IsOver && frameInd >= 0 && frameInd < CardCount)
            {
                Frame frame = Frames[frameInd];

                if (!frame.IsFlipped)
                {
                    frame.IsFlipped = true;
                    bool isDeathCard = false;

                    switch(frame.Color)
                    {
                        case CardColor.Blue:
                            RemainingBlueCards--;
                            break;
                        case CardColor.Red:
                            RemainingRedCards--;
                            break;
                        case CardColor.Black:
                            isDeathCard = true;
                            break;
                    }

                    if (CheckEndConditions(isDeathCard))
                    {
                        return;
                    }

                    if (!CardColorMatchesTeamColor(frame.Color, CurrentTeam))
                    {
                        CurrentTeam = GetOtherTeam(CurrentTeam);
                    }
                }
            }
        }

        public void Reset()
        {
            FrameValueGetter.Reset();
            StartNewGame();
        }

        public string GetReadableGameState()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Current Team: " + CurrentTeam);
            sb.AppendLine("Remaining Blue Cards: " + RemainingBlueCards);
            sb.AppendLine("Remaining Red Cards: " + RemainingRedCards);

            sb.AppendLine("Remaining Cards:");
            for (int i = 0; i < CardCount; i++)
            {
                var frame = Frames[i];
                if (!frame.IsFlipped)
                {
                    sb.AppendLine("Card " + i + ", Color: " + frame.Color + ", Value: " + frame.Value);
                }
            }

            if (IsOver)
            {
                sb.AppendLine("Game is over. " + Winner + " is the Victor!");
            }

            return sb.ToString();
        }

        private void StartNewGame()
        {
            CurrentTeam = GetRandomTeam();
            RemainingBlueCards = RemainingRedCards = CardsPerTeam;
            if (CurrentTeam == Team.Blue)
            {
                RemainingBlueCards++;
            }
            else
            {
                RemainingRedCards++;
            }

            Frames = CreateFrames(FrameValueGetter);
        }

        private bool CheckEndConditions(bool deathCardFlipped)
        {
            if (deathCardFlipped)
            {
                IsOver = true;
                Winner = GetOtherTeam(CurrentTeam);
                return true;
            }

            if (RemainingBlueCards == 0)
            {
                IsOver = true;
                Winner = Team.Blue;
                return true;
            }
            
            if (RemainingRedCards == 0)
            {
                IsOver = true;
                Winner = Team.Red;
                return true;
            }

            return false;
        }

        private Frame[] CreateFrames(IFrameValueGetter fvGetter)
        {
            Frame[] frames = new Frame[CardCount];

            List<CardColor> colors = new List<CardColor>();
            colors.AddRange(Enumerable.Repeat(CardColor.Blue, RemainingBlueCards));
            colors.AddRange(Enumerable.Repeat(CardColor.Red, RemainingRedCards));
            colors.AddRange(Enumerable.Repeat(CardColor.Neutral, NeutralCards));
            colors.AddRange(Enumerable.Repeat(CardColor.Black, DeathCards));

            Random rnd = new Random((int)DateTime.Now.Ticks);

            for (int i = CardCount; i > 0; i--)
            {
                int ind = (rnd.Next() % i);
                CardColor color = colors[ind];
                colors.RemoveAt(ind);

                frames[i - 1] = new Frame(color, fvGetter.GetNext());
            }

            return frames;
        }

        private Team GetOtherTeam(Team team)
        {
            return team == Team.Blue ? Team.Red : Team.Blue;
        }

        private Team GetRandomTeam()
        {
            Random rnd = new Random((int)DateTime.Now.Ticks);
            return (Team)(rnd.Next() % 2);
        }

        private bool CardColorMatchesTeamColor(CardColor cColor, Team team)
        {
            return (cColor == CardColor.Blue && team == Team.Blue)
                || (cColor == CardColor.Red && team == Team.Red);
        }

        public class Frame
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public CardColor Color { get; }
            public bool IsFlipped { get; set; }
            public string Value { get; }

            public Frame(CardColor color, string value)
            {
                Color = color;
                Value = value;
            }
        }
    }

    public enum CardColor
    {
        Neutral = 0,
        Blue = 1,
        Red = 2,
        Black = 3
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Team
    {
        Blue = 0,
        Red = 1
    }
}
