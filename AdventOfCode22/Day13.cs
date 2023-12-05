namespace AdventOfCode22;

public class Day13
{
    [Fact]
    public async Task Part1()
    {
        var input = await File.ReadAllLinesAsync("day13.txt");

        var orderedPacketIndexes = FindOrderedPackets(input).ToArray();
        var result = orderedPacketIndexes.Sum();
        
        Assert.Equal(13, result);
    }

    [Theory]
    // Samples from the intro text
    [InlineData("[1,1,3,1,1]", "[1,1,5,1,1]", -1)]
    [InlineData("[[1],[2,3,4]]", "[[1],4]", -1)]
    [InlineData("[9]", "[[8,7,6]]", 1)]
    [InlineData("[[4,4],4,4]", "[[4,4],4,4,4]", -1)]
    [InlineData("[7,7,7,7]", "[7,7,7]", 1)]
    [InlineData("[]", "[3]", -1)]
    [InlineData("[[[]]]", "[[]]", 1)]
    [InlineData("[1,[2,[3,[4,[5,6,7]]]],8,9]", "[1,[2,[3,[4,[5,6,0]]]],8,9]", 1)]
    // Some other input
    [InlineData("[[2],[7]]", "[[2, 6]]", -1)]
    [InlineData("[[],[2,7]]", "[[2],[6]]", -1)]
    [InlineData("[2,7]", "[2,[6]]", 1)]
    [InlineData("[[2,7], 1]", "[[2,7], 5]", -1)]
    [InlineData("[[2,7], 9]", "[[2,7], 5]", 1)]
    // Data from the actual input data
    [InlineData("[[[[8,6]],[8,[7],[6,7,6,2,4]],10,[[1,7,9],7,[7,9]]],[[4,[],[10,5],[5,4,7],5],8,9,[[5,3,3,6,9],[9,5,10],8],[[0,6,9],[8],4,6,8]]]", "[[[],3,[[10,6,9,6],[6,8,7],[1,2]],8]]", 1)]
    [InlineData("[[4,[4],[[10,7,2],[1,6,5,7,4],[7,3,3,1,5],[]],1],[[],[[7,6,3]],5,5]]", "[[[10]]]", 1)]
    [InlineData("[[[[7,9]],[[0,9]],0,[[2,1,1,2,9],4],[[5]]],[]]", "[[8]]", -1)]
    [InlineData("[[9,[[5,4,5,1,10]]],[[[6,9],3,[0,2]],5,6,[3,4,[2,2],10],[10,5,9]],[[[10,3,8],[],[6,9]],4]]", "[[[4,[7],[1,1,4,3,8],[3],[]],[[5,10,2,1],9,[2],6,7],4,[],[[10],[6,3,1,7]]],[4,[6],[[6],[6,6]]],[[[0,2,5],[3,5,10,7],[10,7,2,9],[],[0]],[[9,9,8],[]],[10,[2,10,7,7,3],4],[2,7]]]", 1)]
    [InlineData("[[[],2,2,9,[[9],[1,10,5,1]]],[[[],9,5,[8]],10,9,[[1,4,0],[5]]]]", "[[5],[3],[],[9,[[7,2,0,9,3],[6,8,6],[]]],[[[6],3],6,[[2,8],8]]]", -1)]
    [InlineData("[[]]", "[[[5,4],5]]", -1)]
    [InlineData("[[7,7,5]]", "[[[[4,7,3],[6,0],1,6],[6,1,4,5]],[]]", 1)]
    [InlineData("[[[[4,7,0,9,8],10,7,2]],[6,6,9],[[2,10],8,9,1,[1,[0,4]]],[[[2,0,2],[]]]]", "[[[[]],[],3],[],[[[],[8]],[[4,7,3,7,10],3,5]],[[9,[10,5,8,9,1]],0],[[[6,7,5,4,10],4,6,0,[8,3,1,2,7]],5,[10,1,5],10,0]]", 1)]
    [InlineData("[[],[[3,8,2]]]", "[[8,4],[5],[3]]", -1)]
    [InlineData("[[0,1,5,[[],[6,6,8,0],[0,2,5,9,9],[3,2,7],[5,2]],10],[],[[8,8],[1,3],3,2],[[[4,0],[]],[[4,1,2],[2],6,9,6],1,10,[[7],[0,7],5]],[4]]", "[[[],[6,[5,9,6,0,8],2,9,[4,2]],7,[9,6,0,10]]]", 1)]
    [InlineData("[[],[],[],[9,[[1],[6,3,4,9]],10,8]]", "[[1,[[2],10,[0,9,3]]]]", -1)]
    [InlineData("[[0,1,4,4],[[7,[6],[2,5,1,3],5],[[8,3,7],[6],[],8],8,1,[9,0,[]]],[[0,[3,9,3,5],[2,0,9,0,7],6]],[[[3,3,1,10,9]],[[0,7,1],8,[10,0],8],[[9,3,5,4],5,[0],5],8,[[3,6,10],0,2,6]]]", "[[[8,[1,2,10,9],2,[8,4,4]],[3,[5,5]],4],[[],2,[],6,3],[[10,7],[],7,[8,6,[5],4,3],2]]", -1)]
    [InlineData("[[[[],1,2],[[7,7,7],6,[1,7,1]]],[4,8],[]]", "[[3,1,[1,6,[9,9,5],5],[[1,7,0,6],[10,8,10],[3,8],[6,2,7,5]]]]", -1)]
    [InlineData("[[1]]", "[[[3],[[9,4,10,10],[8,9,1],[],[0,3,10,5]],[8,[]]],[[[10,6,0],2,1],5,0,[7],[[9]]],[[[],[9,9,3,2,3],5]]]", -1)]
    [InlineData("[[],[7,2],[5,[9,[]],1,[4,6,5],[[9,10,8,3],6]]]", "[[[[]],0,[1,8],[5,2,7,2],6],[],[[],[[2,1,3,9],6,9,5,3],2],[[[10],[0],[4,0,3,6,8]],4],[9]]", -1)]
    [InlineData("[[],[],[],[]]", "[[],[[3,6],6,[6],[[3,10,4],6],[]]]", -1)]
    
    [InlineData("[]", "[[[[0,4,6,7],3],2],[5,[10,[8],[4,9,4],[],5],6,[[0,6],[4,7,7],[],10,[]]]]", -1)]
    [InlineData("[[],[9,[[],3,4],[],3,[1,[8,5,1,9,7],[1,9,3,2],[2],4]],[[1,4,1,[0]],[[6,6,3,6],[2,2,6,9,5],0]],[[3],[]]]", "[[],[[[2,8,10,3]],10,[[5,3],4,8,[8]]],[1,1,[[]]],[],[]]", 1)]
    [InlineData("[[7,[]]]", "[[[],9,[[0,9,1],[6],9,[8,4,10,4,7],[6]]],[3,[2,[0,5],7,9],2],[[[9],7,[1,7],9]]]", 1)]
    [InlineData("[[3,8,[[3,4,1,2,3]],[0,[1],[7,3,8,10,9]],[[2,10,7]]],[[[0,3,8,9],[0,6,8],8,1,2],7,[9,0,7,[9,5,0]],[[]],[[5,3,6]]]]", "[[10,[]],[2,[4],[]],[[[0],[0,6,5],[7,4,3,2,3]]],[]]", 1)]
    [InlineData("[[[]],[],[[[8,3,5,4,4],[9,5,4],[2],[9,4,3],[3,7,3,9]]],[4,[3,[]],0,[[10,9,0],2,8,[1],[]]]]", "[[],[7,[]]]", 1)]
    [InlineData("[[4,0,[[8,0,10,7],10,1,[10,1,8,10],[]],4,10],[7,1],[8],[7,[[4,0,7,1]],[7],[]],[]]", "[[8,[[10,8,4,7,10]],[[10,2]],[5],[[1,5,2,6]]],[[[4,4,3,4,4],6],9,[6],[0,[9,6,5,1],[9],[8,5,0],[0]]],[8,5,[8,[2,6,0,3,4],7,4],8,[6,[4,0,10],[]]],[[[8],[0,2,8,0]],[6,[1]],[],1],[]]", -1)]
    [InlineData("[[2],[[[8]],6],[2,[[7],10,0,[8,7]],9,[9]],[]]", "[[2,[[4,9],4,[],9]],[[1,[8,7,3],0,2],2],[[],5,2],[0,10]]", -1)]
    public void Part1Samples(string left, string right, int expectedResult)
    {
        Assert.Equal(expectedResult, Compare(Parse(left), Parse(right)));
    }

    private IEnumerable<int> FindOrderedPackets(string[] input)
    {
        List<(IPacketItem left, IPacketItem right)> packetGroups = new ();
        for (int i = 0; i < input.Length; i+=3)
        {
            packetGroups.Add(new (Parse(input[i]), Parse(input[i+1])));
        }
        
        for (int index = 0; index < packetGroups.Count; index++)
        {
            if (Compare(packetGroups[index].left, packetGroups[index].right) < 0)
            {
                // index numbers in the example start at 1
                yield return index + 1;
            }
        }
    }
    
    static int Compare(IPacketItem left, IPacketItem right)
    {
        if (left is Digit dl)
        {
            if (right is Digit dr)
            {
                return dl.digit.CompareTo(dr.digit);
            }

            if (right is PacketList rl)
            {
                return Compare(new PacketList() { dl }, rl);
            }
        } else if (left is PacketList ll)
        {
            if (right is Digit dr)
            {
                return Compare(ll, new PacketList() { dr });
            }

            if (right is PacketList rl)
            {
                for (int i = 0; i < ll.Count; i++)
                {
                    if (i >= rl.Count)
                    {
                        // right list ran out of items before left one, left is bigger
                        return 1;
                    }

                    var comparison = Compare(ll[i], rl[i]);
                    if (comparison == 0)
                    {
                        continue;
                    }

                    return comparison;
                }

                if (ll.Count == rl.Count)
                {
                    // both lists are equal in length and values
                    return 0;
                }
                // left list ran out of items before right list
                return -1;
            }
        }

        throw new Exception("invalid");
    }
    
    static IPacketItem Parse(string line)
    {
        PacketList currentItems = new();
        for (int i = 1; i < line.Length - 1; i++)
        {
            var c = line[i];
            if (char.IsDigit(c))
            {
                currentItems.Add(new Digit(int.Parse(c + "")));
            } else if (c == ',')
            {
                // continue
            } else if (c == '[')
            {
                var subList = new PacketList();
                currentItems.Add(subList);
                subList.Parent = currentItems;
                currentItems = subList;
            }
            else if (c == ']')
            {
                currentItems = currentItems.Parent;
            }
        }

        return currentItems;
    }

    interface IPacketItem
    {
    }

    record Digit(int digit) : IPacketItem
    {
    }

    class PacketList : List<IPacketItem>, IPacketItem
    {
        public PacketList Parent { get; set; }
    }
}