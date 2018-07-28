using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeFrames;

namespace CodeFramesPlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game(new TestFrameValueGenerator());
            Console.WriteLine(game.GetReadableGameState());
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int guess))
                {
                    game.Guess(guess);
                    Console.WriteLine(game.GetReadableGameState());
                }
            }
        }
    }
}
