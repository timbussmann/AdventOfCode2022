namespace AdventOfCode22;

using Xunit;

public class Day2
{
    const int Score_Loss = 0;
    const int Score_Draw = 3;
    const int Score_Win = 6;

    [Fact]
    void Part1()
    {
        var input = File.ReadAllLines("day2.txt");
        int totalScore = 0;
        foreach (var line in input)
        {
            var enemy = line[0];
            var user = line[^1];
            var matchScore = (enemy, user) switch
            {
                ('A', 'X') => Score_Draw,
                ('A', 'Y') => Score_Win,
                ('B', 'Y') => Score_Draw,
                ('B', 'Z') => Score_Win,
                ('C', 'Z') => Score_Draw,
                ('C', 'X') => Score_Win,
                _ => 0,
            };

            var choiceScore = user switch
            {
                'X' => 1,
                'Y' => 2,
                'Z' => 3,
                _ => throw new ArgumentOutOfRangeException()
            };
            var score = matchScore + choiceScore;
            totalScore += score;
        }

        Assert.Equal(15691, totalScore);
    }

    [Fact]
    void Part2()
    {
        var input = File.ReadAllLines("day2.txt");
        int totalScore = 0;
        foreach (var line in input)
        {
            var enemy = line[0];
            var expectedOutcome = line[^1];
            var matchScore = expectedOutcome switch
            {
                'X' => Score_Loss,
                'Y' => Score_Draw,
                'Z' => Score_Win,
                _ => throw new ArgumentOutOfRangeException()
            };
            var choiceScore = (ParseOpponentHand(enemy), matchScore) switch
            {
                (Hand.Rock, Score_Loss) => Hand.Scissor,
                (Hand.Rock, Score_Draw) => Hand.Rock,
                (Hand.Rock, Score_Win) => Hand.Paper,
                (Hand.Paper, Score_Loss) => Hand.Rock,
                (Hand.Paper, Score_Draw) => Hand.Paper,
                (Hand.Paper, Score_Win) => Hand.Scissor,
                (Hand.Scissor, Score_Loss) => Hand.Paper,
                (Hand.Scissor, Score_Draw) => Hand.Scissor,
                (Hand.Scissor, Score_Win) => Hand.Rock,
                _ => throw new ArgumentOutOfRangeException()
            };

            int score = matchScore + (int)choiceScore;
            totalScore += score;
        }

        Assert.Equal(12989, totalScore);
    }

    static Hand ParseOpponentHand(char c)
    {
        return c switch
        {
            'A' => Hand.Rock,
            'B' => Hand.Paper,
            'C' => Hand.Scissor,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
    }
    enum Hand
    {
        Rock = 1,
        Paper = 2,
        Scissor = 3
    }
}