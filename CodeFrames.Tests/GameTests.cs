using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CodeFrames.Tests
{
    public class GameTests
    {
        [Fact]
        public void Picking_Your_Teams_Card_Doesnt_End_Your_Turn()
        {
            var frameGetter = new Mock<IFrameValueGetter>();
            frameGetter.Setup(x => x.GetNext()).Returns("value");
            Game game = new Game(frameGetter.Object);

            Team teamBeforeGuess = game.CurrentTeam;
            int goodGuess = game.Frames.ToList().FindIndex(f => IsCardForTeam(teamBeforeGuess, f.Color));
            game.Guess(goodGuess);

            game.CurrentTeam.Should().Be(teamBeforeGuess);
        }

        private bool IsCardForTeam(Team team, CardColor cardColor)
        {
            if (team == Team.Blue && cardColor == CardColor.Blue
                || team == Team.Red && cardColor == CardColor.Red)
            {
                return true;
            }
            return false;
        }
    }
}
